using System;
using System.Linq;
using NUnit.Framework;
using Dapper;

namespace Majorsilence.Vpn.Site.TestsFast.LiveSite;

public class GenerateCertsTest
{
    public GenerateCertsTest()
    {
    }

    private readonly string emailAddress = "testgeneratecerts@majorsilence.com";
    private int userid;
    private int regionid;
    private int vpnseverid;

    [SetUp()]
    public void Setup()
    {
        var peterAccount = new Logic.Accounts.CreateAccount(
            new Logic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Bobby",
                Lastname = "Smith",
                Password = "Password54",
                PasswordConfirm = "Password54",
                BetaKey = ""
            }
            , false, Logic.InitializeSettings.Email);

        userid = peterAccount.Execute();

        var region = new Logic.Admin.Regions();
        regionid = region.Insert("Test region", true);

        var vpnserver = new Logic.Admin.VpnServers();
        vpnseverid = vpnserver.Insert("localhost", 5678, "a fake vpnserver for testing", regionid, true);
    }

    [TearDown()]
    public void Cleanup()
    {
        using (var cn = Logic.InitializeSettings.DbFactory)
        {
            cn.Open();
            cn.Execute("DELETE FROM ActionLog WHERE UserId=@UserId", new { UserId = userid });
            cn.Execute("DELETE FROM UserOpenVpnCerts WHERE UserId=@UserId", new { UserId = userid });
            cn.Execute("DELETE FROM VpnServers WHERE Id = @Id", new { Id = vpnseverid });
            cn.Execute("DELETE FROM Regions WHERE Id = @Id", new { Id = regionid });
            cn.Execute("DELETE FROM UserPayments WHERE UserId = @UserId", new { UserId = userid });
            cn.Execute("DELETE FROM Users WHERE Email = @email", new { email = emailAddress });
        }
    }


    [Test()]
    public void GenerateCertHappyPath()
    {
        var pay = new Logic.Payments.Payment(userid);
        pay.SaveUserPayment(5, DateTime.UtcNow, Logic.Helpers.SiteInfo.MonthlyPaymentId);

        using (var sshClient = new Logic.Ssh.FakeSsh(Logic.Ssh.FakeSsh.TestingScenerios.OpenVpnHappyPath))
        using (var sshRevokeClient = new Logic.Ssh.FakeSsh(Logic.Ssh.FakeSsh.TestingScenerios.OpenVpnHappyPath))
        using (var sftpClient = new Logic.Ssh.FakeSftp())
        {
            var vpn = new Logic.OpenVpn.CertsOpenVpnGenerateCommand(userid, vpnseverid,
                sshClient, sshRevokeClient, sftpClient);

            vpn.Execute();
        }
    }

    [Test()]
    public void InactiveAccount()
    {
        using (var sshClient = new Logic.Ssh.FakeSsh(Logic.Ssh.FakeSsh.TestingScenerios.OpenVpnHappyPath))
        using (var sshRevokeClient = new Logic.Ssh.FakeSsh(Logic.Ssh.FakeSsh.TestingScenerios.OpenVpnHappyPath))
        using (var sftpClient = new Logic.Ssh.FakeSftp())
        {
            var vpn = new Logic.OpenVpn.CertsOpenVpnGenerateCommand(userid, vpnseverid,
                sshClient, sshRevokeClient, sftpClient);

            Assert.Throws<Logic.Exceptions.AccountNotActiveException>(() => vpn.Execute());
        }
    }
}