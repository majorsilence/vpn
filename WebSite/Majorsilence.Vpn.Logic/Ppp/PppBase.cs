using System;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Linq;
using System.Data;
using System.IO;

namespace Majorsilence.Vpn.Logic.Ppp;

public abstract class PppBase
{
    protected Poco.Users userData;
    protected Poco.VpnServers vpnData;
    protected string userRequestedPassword;
    private Ssh.ISsh sshNewServer;
    private Ssh.ISsh sshRevokeServer;

    protected PppBase()
    {
    }

    protected PppBase(int userId, int vpnServerId, Ssh.ISsh sshNewServer, Ssh.ISsh sshRevokeServer)
    {
        using (var db = InitializeSettings.DbFactory)
        {
            userData = db.Get<Poco.Users>(userId);
            vpnData = db.Get<Poco.VpnServers>(vpnServerId);
        }

        this.sshNewServer = sshNewServer;
        this.sshRevokeServer = sshRevokeServer;
        userRequestedPassword = GeneratePassword();
    }

    private bool IsActiveAccount()
    {
        var pay = new Payments.Payment(userData.Id);
        return !pay.IsExpired();
    }

    public void AddUser()
    {
        RevokeUser();

        if (IsActiveAccount() == false)
            throw new Exceptions.AccountNotActiveException(
                "Do generate a new pptp or ipsec user you first activate your account by making a payment.");


        // Configure DNS servers to use when clients connect to this PPTP server

        sshNewServer.Login(vpnData.Address);
        sshNewServer.WriteLine("sudo su");

        AddUserImplementation(sshNewServer);

        sshNewServer.WriteLine("exit");
        // give server a chance to finish
        System.Threading.Thread.Sleep(2000);
        var output = sshNewServer.Read();


        SaveUserInfo();
    }

    protected abstract void AddUserImplementation(Ssh.ISsh sshClient);

    public void RevokeUser()
    {
        // we should only revoke if we have records indicating the user has an account on this server.
        using (var db = InitializeSettings.DbFactory)
        {
            var certData = db.Query<Poco.UserPptpInfo>("SELECT * FROM UserPptpInfo wHERE UserId=@UserId",
                new { UserId = userData.Id });
            if (certData.Count() == 0) return;
        }

        // remove user from pptp server config
        sshRevokeServer.Login(vpnData.Address);
        sshRevokeServer.WriteLine("sudo su");
        RevokeUserImplementation(sshRevokeServer);
        sshRevokeServer.WriteLine("exit");

        // give server a chance to finish
        System.Threading.Thread.Sleep(2000);
        var output = sshRevokeServer.Read();

        // TODO: Update UserPptpInfo table
    }

    protected abstract void RevokeUserImplementation(Ssh.ISsh sshClient);

    protected void SaveUserInfo()
    {
        using (var db = InitializeSettings.DbFactory)
        {
            var data = db.Query<Poco.UserPptpInfo>("SELECT * FROM UserPptpInfo wHERE UserId=@UserId",
                new { UserId = userData.Id });

            if (data.Count() == 0)
            {
                var newData = new Poco.UserPptpInfo(userData.Id, false, DateTime.UtcNow, vpnData.Id,
                    userRequestedPassword);
                db.Insert(newData);
            }
            else
            {
                data.First().Expired = false;
                data.First().CreateTime = DateTime.UtcNow;
                data.First().VpnServersId = vpnData.Id;
                data.First().Password = userRequestedPassword;
                db.Update(data.First());
            }
        }
    }

    private string GeneratePassword()
    {
        return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
    }
}