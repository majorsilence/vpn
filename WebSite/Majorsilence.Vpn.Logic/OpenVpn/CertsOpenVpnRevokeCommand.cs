using System.Linq;
using System.Threading;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.Ssh;
using Majorsilence.Vpn.Poco;
using InvalidDataException = System.IO.InvalidDataException;

namespace Majorsilence.Vpn.Logic.OpenVpn;

public class CertsOpenVpnRevokeCommand : ICommand
{
    private readonly ISsh sshClient;
    private readonly Users userData;
    private readonly DatabaseSettings _dbSettings;
    private readonly ActionLog _actionLog;
    private CertsOpenVpnRevokeCommand()
    {
    }

    public CertsOpenVpnRevokeCommand(int userId, ISsh sshClient, DatabaseSettings dbSettings,
        ActionLog actionLog)
    {
        _dbSettings = dbSettings;
        _actionLog = actionLog;
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();
            userData = db.Get<Users>(userId);
        }

        this.sshClient = sshClient;
    }

    public void Execute()
    {
        var certName = "";
        VpnServers vpnData = null;
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();
            var certData = db.Query<UserOpenVpnCerts>("SELECT * FROM UserOpenVpnCerts WHERE UserId=@UserId",
                new { UserId = userData.Id });
            if (certData.Count() == 1)
            {
                certName = certData.First().CertName;
                vpnData = db.Get<VpnServers>(certData.First().VpnServersId);
            }
            else if (certData.Count() >= 1)
            {
                throw new InvalidDataException("Invalid data in the UserOpenVpnCerts for user " + userData.Id);
            }
        }

        if (certName == "") return;

        _actionLog.Log("OpenVpn Revoke Start", userData.Id);

        RevokeUserCert(vpnData.Address, certName);

        _actionLog.Log("OpenVpn Revoke Finished", userData.Id);
    }

    /// <summary>
    ///     Revoke a users vpn certificate.  This is generally used when they close their account
    ///     or stop making payments.
    /// </summary>
    /// <param name="certName"></param>
    private void RevokeUserCert(string host, string certName)
    {
        // Run Command on ssh server

        sshClient.Login(host);

        sshClient.WriteLine("sudo su");
        sshClient.WriteLine("cd /etc/openvpn/easy-rsa/");
        sshClient.WriteLine("source vars");
        sshClient.WriteLine(string.Format("./revoke-full {0}", certName));

        var count = 0;
        var output = "";
        while (output.ToLower().Contains("error 23 at 0 depth lookup:certificate revoked") == false)
        {
            if (count > 20) throw new SshException("Error revoking client cert on vpn server");

            if (output.ToLower().Contains("no such file or directory"))
                throw new SshException("Error revoking client cert on vpn server");

            if (output.ToLower().Contains("already revoked")) return;

            output += sshClient.Read();
            Thread.Sleep(1000);
            count++;
        }

        sshClient.WriteLine("exit");
    }
}