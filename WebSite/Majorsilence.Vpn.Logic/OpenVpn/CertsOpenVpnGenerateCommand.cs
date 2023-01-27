using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Logic.OpenVpn;

public class CertsOpenVpnGenerateCommand : ICommand
{
    private readonly Poco.Users userData;
    private readonly Poco.VpnServers vpnData;
    private Ssh.ISsh sshClientNewServer;
    private Ssh.ISsh sshClientRevokeServer;
    private Ssh.ISftp sftpClient;

    private CertsOpenVpnGenerateCommand()
    {
    }

    public CertsOpenVpnGenerateCommand(int userId, int vpnServerId, Ssh.ISsh sshClientNewServer,
        Ssh.ISsh sshClientRevokeServer, Ssh.ISftp sftpClient)
    {
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            userData = db.Get<Poco.Users>(userId);
            vpnData = db.Get<Poco.VpnServers>(vpnServerId);
        }

        this.sshClientNewServer = sshClientNewServer;
        this.sshClientRevokeServer = sshClientRevokeServer;
        this.sftpClient = sftpClient;
    }

    private bool IsActiveAccount()
    {
        var pay = new Payments.Payment(userData.Id);
        return !pay.IsExpired();
    }

    public void Execute()
    {
        if (IsActiveAccount() == false)
            throw new Exceptions.AccountNotActiveException(
                "To generate a new open vpn cert you first activate your account by making a payment.");

        ActionLog.Log_BackgroundThread(string.Format("OpenVpn Generate Command Start - {0}", vpnData.Description),
            userData.Id);

        ulong num = 0;
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            using (var txn = db.BeginTransaction())
            {
                num = Counters.GetSetVpnNum(txn, db);
                txn.Commit();
            }
        }

        var randomString = Guid.NewGuid().ToString().Split('-')[0];
        var certName = string.Format("{0}{1}-{2}", num, randomString, userData.Email);

        // First things first, revoke any old certificates on the server
        var revoke = new CertsOpenVpnRevokeCommand(userData.Id, sshClientRevokeServer);
        revoke.Execute();

        // now generate the new certificate
        CreateUserCerts(certName);

        ActionLog.Log_BackgroundThread("OpenVpn Generate Command Finished", userData.Id);
    }


    /// <summary>
    /// Create new vpn certificates for users once they have made their payments
    /// </summary>
    /// <param name="certName"></param>
    private void CreateUserCerts(string certName)
    {
        var crt_str_orig = string.Format("/etc/openvpn/easy-rsa/keys/{0}.crt", certName);
        var key_str_orig = string.Format("/etc/openvpn/easy-rsa/keys/{0}.key", certName);
        var csr_str_orig = string.Format("/etc/openvpn/easy-rsa/keys/{0}.csr", certName);

        var crt_str_moved = string.Format("/etc/openvpn/downloadclientcerts/{0}.crt", certName);
        var key_str_moved = string.Format("/etc/openvpn/downloadclientcerts/{0}.key", certName);
        var csr_str_moved = string.Format("/etc/openvpn/downloadclientcerts/{0}.csr", certName);

        CreateAccount(certName, crt_str_orig, key_str_orig, csr_str_orig, crt_str_moved,
            key_str_moved, csr_str_moved);
        DownloadCerts(certName, crt_str_moved, key_str_moved);
        RemoveTempFiles(crt_str_moved, key_str_moved, csr_str_moved);
    }

    private void CreateAccount(string certName, string crt_str_orig, string key_str_orig, string csr_str_orig,
        string crt_str_moved, string key_str_moved, string csr_str_moved)
    {
        sshClientNewServer.Login(vpnData.Address);

        sshClientNewServer.WriteLine("sudo su");
        sshClientNewServer.WriteLine("cd /etc/openvpn/easy-rsa/");
        sshClientNewServer.WriteLine("source vars");
        sshClientNewServer.WriteLine(string.Format("KEY_CN=client-{0} ./pkitool {0}", certName));
        var count = 0;
        var output = "";

        while (output.ToLower().Contains("certificate is to be certified until") == false)
        {
            if (count > 20) throw new Exceptions.SshException("Error creating client cert on vpn server");

            output += sshClientNewServer.Read();
            Console.WriteLine("ssh output: " + output);
            System.Threading.Thread.Sleep(1000);
            count++;
        }

        if (output.ToLower().Contains("txt_db error number 2"))
            // see http://blog.kenyap.com.au/2012/07/txtdb-error-number-2-when-generating.html
            throw new Exceptions.SshException("TXT_DB error number 2");
        sshClientNewServer.WriteLine(string.Format("cp {0} /etc/openvpn/downloadclientcerts/", crt_str_orig));
        sshClientNewServer.WriteLine(string.Format("cp {0} /etc/openvpn/downloadclientcerts/", key_str_orig));
        sshClientNewServer.WriteLine(string.Format("cp {0} /etc/openvpn/downloadclientcerts/", csr_str_orig));
        sshClientNewServer.WriteLine(string.Format("chmod 644 {0}", crt_str_moved));
        sshClientNewServer.WriteLine(string.Format("chmod 644 {0}", key_str_moved));
        sshClientNewServer.WriteLine(string.Format("chmod 644 {0}", csr_str_moved));
        sshClientNewServer.WriteLine("exit");
        // give server a chance to move files
        System.Threading.Thread.Sleep(2000);
    }

    private void DownloadCerts(string certName, string crt_str_moved, string key_str_moved)
    {
        sftpClient.Login(vpnData.Address);
        // TODO: save cert and keys in database linked to user
        var ca = new MemoryStream();
        sftpClient.DownloadFile("/etc/openvpn/ca.crt", ca);
        var crt = new MemoryStream();
        sftpClient.DownloadFile(crt_str_moved, crt);
        var key = new MemoryStream();
        sftpClient.DownloadFile(key_str_moved, key);
        SaveUserCert(certName, ca.ToArray(), crt.ToArray(), key.ToArray(), false);
    }

    private void RemoveTempFiles(string crt_str_moved, string key_str_moved, string csr_str_moved)
    {
        // Run Command on ssh server

        sshClientNewServer.Login(vpnData.Address);

        sshClientNewServer.WriteLine("sudo su");
        sshClientNewServer.WriteLine(string.Format("rm -rf {0}", crt_str_moved));
        sshClientNewServer.WriteLine(string.Format("rm -rf {0}", key_str_moved));
        sshClientNewServer.WriteLine(string.Format("rm -rf {0}", csr_str_moved));
        sshClientNewServer.WriteLine("exit");
        System.Threading.Thread.Sleep(2000);
        var output = sshClientNewServer.Read();
    }

    private void SaveUserCert(string certName, byte[] certCa, byte[] certCrt,
        byte[] certKey, bool expired)
    {
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            // TODO: how does this work, id is user id not UserOpenVpnCerts id
            var certData = db.Query<Poco.UserOpenVpnCerts>("SELECT * FROM UserOpenVpnCerts WHERE UserId=@UserId",
                new { UserId = userData.Id });

            if (!certData.Any())
            {
                var certDataNew = new Poco.UserOpenVpnCerts(userData.Id, certName, certCa, certCrt, certKey, expired,
                    DateTime.UtcNow, vpnData.Id);
                db.Insert(certDataNew);
            }
            else
            {
                certData.First().CertCa = certCa;
                certData.First().CertCrt = certCrt;
                certData.First().CertKey = certKey;
                certData.First().CertName = certName;
                certData.First().Expired = expired;
                certData.First().CreateTime = DateTime.UtcNow;
                certData.First().VpnServersId = vpnData.Id;
                db.Update(certData.First());
            }
        }
    }
}