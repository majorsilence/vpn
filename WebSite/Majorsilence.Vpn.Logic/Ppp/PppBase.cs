using System;
using System.Linq;
using System.Threading;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.Payments;
using Majorsilence.Vpn.Logic.Ssh;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Logic.Ppp;

public abstract class PppBase
{
    private readonly DatabaseSettings _dbSettings;
    private readonly ISsh sshNewServer;
    private readonly ISsh sshRevokeServer;
    protected Users userData;
    protected string userRequestedPassword;
    protected VpnServers vpnData;

    protected PppBase()
    {
    }

    protected PppBase(int userId, int vpnServerId, ISsh sshNewServer, ISsh sshRevokeServer,
        DatabaseSettings dbSettings)
    {
        _dbSettings = dbSettings;
        using (var db = _dbSettings.DbFactory)
        {
            userData = db.Get<Users>(userId);
            vpnData = db.Get<VpnServers>(vpnServerId);
        }

        this.sshNewServer = sshNewServer;
        this.sshRevokeServer = sshRevokeServer;
        userRequestedPassword = GeneratePassword();
    }

    private bool IsActiveAccount()
    {
        var pay = new Payment(userData.Id, _dbSettings);
        return !pay.IsExpired();
    }

    public void AddUser()
    {
        RevokeUser();

        if (IsActiveAccount() == false)
            throw new AccountNotActiveException(
                "Do generate a new pptp or ipsec user you first activate your account by making a payment.");


        // Configure DNS servers to use when clients connect to this PPTP server

        sshNewServer.Login(vpnData.Address);
        sshNewServer.WriteLine("sudo su");

        AddUserImplementation(sshNewServer);

        sshNewServer.WriteLine("exit");
        // give server a chance to finish
        Thread.Sleep(2000);
        var output = sshNewServer.Read();


        SaveUserInfo();
    }

    protected abstract void AddUserImplementation(ISsh sshClient);

    public void RevokeUser()
    {
        // we should only revoke if we have records indicating the user has an account on this server.
        using (var db = _dbSettings.DbFactory)
        {
            var certData = db.Query<UserPptpInfo>("SELECT * FROM UserPptpInfo wHERE UserId=@UserId",
                new { UserId = userData.Id });
            if (certData.Count() == 0) return;
        }

        // remove user from pptp server config
        sshRevokeServer.Login(vpnData.Address);
        sshRevokeServer.WriteLine("sudo su");
        RevokeUserImplementation(sshRevokeServer);
        sshRevokeServer.WriteLine("exit");

        // give server a chance to finish
        Thread.Sleep(2000);
        var output = sshRevokeServer.Read();

        // TODO: Update UserPptpInfo table
    }

    protected abstract void RevokeUserImplementation(ISsh sshClient);

    protected void SaveUserInfo()
    {
        using (var db = _dbSettings.DbFactory)
        {
            var data = db.Query<UserPptpInfo>("SELECT * FROM UserPptpInfo wHERE UserId=@UserId",
                new { UserId = userData.Id });

            if (data.Count() == 0)
            {
                var newData = new UserPptpInfo(userData.Id, false, DateTime.UtcNow, vpnData.Id,
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