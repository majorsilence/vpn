using System;
using System.Linq;
using NUnit.Framework;
using Dapper;

namespace Majorsilence.Vpn.Site.TestsFast.LiveSite
{
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

            var peterAccount = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
                                   new Majorsilence.Vpn.Logic.Accounts.CreateAccountInfo()
                {
                    Email = emailAddress,
                    EmailConfirm = emailAddress,
                    Firstname = "Bobby",
                    Lastname = "Smith",
                    Password = "Password54",
                    PasswordConfirm = "Password54",
                    BetaKey = ""
                }
                , false, Majorsilence.Vpn.Logic.Setup.Email);

            this.userid = peterAccount.Execute();

            var region = new Majorsilence.Vpn.Logic.Admin.Regions();
            regionid = region.Insert("Test region", true);

            var vpnserver = new Majorsilence.Vpn.Logic.Admin.VpnServers();
            vpnseverid = vpnserver.Insert("localhost", 5678, "a fake vpnserver for testing", regionid, true);

        }

        [TearDown()]
        public void Cleanup()
        {
            using (var cn = Majorsilence.Vpn.Logic.Setup.DbFactory)
            {
                cn.Open();
                cn.Execute("DELETE FROM ActionLog WHERE UserId=@UserId", new {UserId = this.userid});
                cn.Execute("DELETE FROM UserOpenVpnCerts WHERE UserId=@UserId", new {UserId = this.userid});
                cn.Execute("DELETE FROM VpnServers WHERE Id = @Id", new {Id = vpnseverid});
                cn.Execute("DELETE FROM Regions WHERE Id = @Id", new {Id = regionid});
                cn.Execute("DELETE FROM UserPayments WHERE UserId = @UserId", new {UserId = this.userid});
                cn.Execute("DELETE FROM Users WHERE Email = @email", new {email = emailAddress});
            }
        }


        [Test()]
        public void GenerateCertHappyPath()
        {

            var pay = new Majorsilence.Vpn.Logic.Payments.Payment(this.userid);
            pay.SaveUserPayment(5, DateTime.UtcNow, Majorsilence.Vpn.Logic.Helpers.SiteInfo.MonthlyPaymentId);

            using (var sshClient = new Majorsilence.Vpn.Logic.Ssh.FakeSsh(Majorsilence.Vpn.Logic.Ssh.FakeSsh.TestingScenerios.OpenVpnHappyPath))
            using (var sshRevokeClient = new Majorsilence.Vpn.Logic.Ssh.FakeSsh(Majorsilence.Vpn.Logic.Ssh.FakeSsh.TestingScenerios.OpenVpnHappyPath))
            using (var sftpClient = new Majorsilence.Vpn.Logic.Ssh.FakeSftp())
            {


                var vpn = new Majorsilence.Vpn.Logic.OpenVpn.CertsOpenVpnGenerateCommand(this.userid, this.vpnseverid, 
                              sshClient, sshRevokeClient, sftpClient);

                vpn.Execute();


            }

        }

        [Test()]
        public void InactiveAccount()
        {
  
            using (var sshClient = new Majorsilence.Vpn.Logic.Ssh.FakeSsh(Majorsilence.Vpn.Logic.Ssh.FakeSsh.TestingScenerios.OpenVpnHappyPath))
            using (var sshRevokeClient = new Majorsilence.Vpn.Logic.Ssh.FakeSsh(Majorsilence.Vpn.Logic.Ssh.FakeSsh.TestingScenerios.OpenVpnHappyPath))
            using (var sftpClient = new Majorsilence.Vpn.Logic.Ssh.FakeSftp())
            {
                var vpn = new Majorsilence.Vpn.Logic.OpenVpn.CertsOpenVpnGenerateCommand(this.userid, this.vpnseverid, 
                                             sshClient, sshRevokeClient, sftpClient);

                Assert.Throws<Majorsilence.Vpn.Logic.Exceptions.AccountNotActiveException>(() => vpn.Execute());
                
            }

        }

    }
}

