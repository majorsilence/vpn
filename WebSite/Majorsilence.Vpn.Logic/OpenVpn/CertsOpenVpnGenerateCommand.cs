using System;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Threading;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.Payments;
using Majorsilence.Vpn.Logic.Ssh;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Logic.OpenVpn;

public class CertsOpenVpnGenerateCommand : ICommand
{
    private readonly Users userData;
    private readonly VpnServers vpnData;
    private readonly ISftp sftpClient;
    private readonly ISsh sshClientNewServer;
    private readonly ISsh sshClientRevokeServer;
    private readonly DatabaseSettings _dbSettings;
    private readonly ActionLog _actionLog;

    private CertsOpenVpnGenerateCommand()
    {
    }

    public CertsOpenVpnGenerateCommand(int userId, int vpnServerId, ISsh sshClientNewServer,
        ISsh sshClientRevokeServer, ISftp sftpClient, DatabaseSettings dbSettings,
        ActionLog actionLog)
    {
        _dbSettings = dbSettings;
        _actionLog = actionLog;
        using (var db = dbSettings.DbFactory)
        {
            db.Open();
            userData = db.Get<Users>(userId);
            vpnData = db.Get<VpnServers>(vpnServerId);
        }

        this.sshClientNewServer = sshClientNewServer;
        this.sshClientRevokeServer = sshClientRevokeServer;
        this.sftpClient = sftpClient;
    }

    public void Execute()
    {
        if (IsActiveAccount() == false)
            throw new AccountNotActiveException(
                "To generate a new open vpn cert you first activate your account by making a payment.");

        _actionLog.Log(string.Format("OpenVpn Generate Command Start - {0}", vpnData.Description),
            userData.Id);

        ulong num = 0;
        using (var db = _dbSettings.DbFactory)
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
        var revoke = new CertsOpenVpnRevokeCommand(userData.Id, sshClientRevokeServer, _dbSettings, _actionLog);
        revoke.Execute();

        // now generate the new certificate
        CreateUserCerts(certName);

        _actionLog.Log("OpenVpn Generate Command Finished", userData.Id);
    }

    private bool IsActiveAccount()
    {
        var pay = new Payment(userData.Id, _dbSettings);
        return !pay.IsExpired();
    }


    /// <summary>
    ///     Create new vpn certificates for users once they have made their payments
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
            if (count > 20) throw new SshException("Error creating client cert on vpn server");

            output += sshClientNewServer.Read();
            Console.WriteLine("ssh output: " + output);
            Thread.Sleep(1000);
            count++;
        }

        if (output.ToLower().Contains("txt_db error number 2"))
            // see http://blog.kenyap.com.au/2012/07/txtdb-error-number-2-when-generating.html
            throw new SshException("TXT_DB error number 2");
        sshClientNewServer.WriteLine(string.Format("cp {0} /etc/openvpn/downloadclientcerts/", crt_str_orig));
        sshClientNewServer.WriteLine(string.Format("cp {0} /etc/openvpn/downloadclientcerts/", key_str_orig));
        sshClientNewServer.WriteLine(string.Format("cp {0} /etc/openvpn/downloadclientcerts/", csr_str_orig));
        sshClientNewServer.WriteLine(string.Format("chmod 644 {0}", crt_str_moved));
        sshClientNewServer.WriteLine(string.Format("chmod 644 {0}", key_str_moved));
        sshClientNewServer.WriteLine(string.Format("chmod 644 {0}", csr_str_moved));
        sshClientNewServer.WriteLine("exit");
        // give server a chance to move files
        Thread.Sleep(2000);
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
        Thread.Sleep(2000);
        var output = sshClientNewServer.Read();
    }

    private void SaveUserCert(string certName, byte[] certCa, byte[] certCrt,
        byte[] certKey, bool expired)
    {
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();
            // TODO: how does this work, id is user id not UserOpenVpnCerts id
            var certData = db.Query<UserOpenVpnCerts>("SELECT * FROM UserOpenVpnCerts WHERE UserId=@UserId",
                new { UserId = userData.Id });

            if (!certData.Any())
            {
                var certDataNew = new UserOpenVpnCerts(userData.Id, certName, certCa, certCrt, certKey, expired,
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