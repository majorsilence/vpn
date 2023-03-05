using System;
using System.Threading.Tasks;
using Dapper;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Admin;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.OpenVpn;
using Majorsilence.Vpn.Logic.Payments;
using Majorsilence.Vpn.Logic.Ssh;
using NUnit.Framework;

namespace Majorsilence.Vpn.Site.TestsFast.LiveSite;

public class GenerateCertsTest
{
    private readonly string emailAddress = "testgeneratecerts@majorsilence.com";
    private int regionid;
    private int userid;
    private int vpnseverid;

    [SetUp]
    public async Task Setup()
    {
        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Bobby",
                Lastname = "Smith",
                Password = "Password54",
                PasswordConfirm = "Password54",
                BetaKey = ""
            }
            , false, DatabaseSettings.Email);

        userid = await peterAccount.ExecuteAsync();

        var region = new Regions();
        regionid = region.Insert("Test region", true);

        var vpnserver = new VpnServers();
        vpnseverid = vpnserver.Insert("localhost", 5678, "a fake vpnserver for testing", regionid, true);
    }

    [TearDown]
    public void Cleanup()
    {
        using (var cn = DatabaseSettings.DbFactory)
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


    [Test]
    public void GenerateCertHappyPath()
    {
        var pay = new Payment(userid);
        pay.SaveUserPayment(5, DateTime.UtcNow, Logic.Helpers.SiteInfo.MonthlyPaymentId);

        using (var sshClient = new FakeSsh(FakeSsh.TestingScenerios.OpenVpnHappyPath))
        using (var sshRevokeClient = new FakeSsh(FakeSsh.TestingScenerios.OpenVpnHappyPath))
        using (var sftpClient = new FakeSftp())
        {
            var vpn = new CertsOpenVpnGenerateCommand(userid, vpnseverid,
                sshClient, sshRevokeClient, sftpClient);

            vpn.Execute();
        }
    }

    [Test]
    public void InactiveAccount()
    {
        using (var sshClient = new FakeSsh(FakeSsh.TestingScenerios.OpenVpnHappyPath))
        using (var sshRevokeClient = new FakeSsh(FakeSsh.TestingScenerios.OpenVpnHappyPath))
        using (var sftpClient = new FakeSftp())
        {
            var vpn = new CertsOpenVpnGenerateCommand(userid, vpnseverid,
                sshClient, sshRevokeClient, sftpClient);

            Assert.Throws<AccountNotActiveException>(() => vpn.Execute());
        }
    }
}