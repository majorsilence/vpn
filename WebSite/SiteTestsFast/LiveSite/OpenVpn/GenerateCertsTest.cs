using System;
using System.Linq;
using NUnit.Framework;
using Dapper;

namespace SiteTestsFast.LiveSite
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

            var peterAccount = new LibLogic.Accounts.CreateAccount(
                                   new LibLogic.Accounts.CreateAccountInfo()
                {
                    Email = emailAddress,
                    EmailConfirm = emailAddress,
                    Firstname = "Bobby",
                    Lastname = "Smith",
                    Password = "Password54",
                    PasswordConfirm = "Password54",
                    BetaKey = ""
                }
                , false, LibLogic.Setup.Email);

            this.userid = peterAccount.Execute();

            var region = new LibLogic.Admin.Regions();
            regionid = region.Insert("Test region", true);

            var vpnserver = new LibLogic.Admin.VpnServers();
            vpnseverid = vpnserver.Insert("localhost", 5678, "a fake vpnserver for testing", regionid, true);

        }

        [TearDown()]
        public void Cleanup()
        {
            using (var cn = LibLogic.Setup.DbFactory)
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

            var pay = new LibLogic.Payments.Payment(this.userid);
            pay.SaveUserPayment(5, DateTime.UtcNow, LibLogic.Helpers.SiteInfo.MonthlyPaymentId);

            using (var sshClient = new LibLogic.Ssh.FakeSsh(LibLogic.Ssh.FakeSsh.TestingScenerios.OpenVpnHappyPath))
            using (var sshRevokeClient = new LibLogic.Ssh.FakeSsh(LibLogic.Ssh.FakeSsh.TestingScenerios.OpenVpnHappyPath))
            using (var sftpClient = new LibLogic.Ssh.FakeSftp())
            {


                var vpn = new LibLogic.OpenVpn.CertsOpenVpnGenerateCommand(this.userid, this.vpnseverid, 
                              sshClient, sshRevokeClient, sftpClient);

                vpn.Execute();


            }

        }

        [Test()]
        [ExpectedException(typeof(LibLogic.Exceptions.AccountNotActiveException))]
        public void InactiveAccount()
        {
  
            using (var sshClient = new LibLogic.Ssh.FakeSsh(LibLogic.Ssh.FakeSsh.TestingScenerios.OpenVpnHappyPath))
            using (var sshRevokeClient = new LibLogic.Ssh.FakeSsh(LibLogic.Ssh.FakeSsh.TestingScenerios.OpenVpnHappyPath))
            using (var sftpClient = new LibLogic.Ssh.FakeSftp())
            {
                var vpn = new LibLogic.OpenVpn.CertsOpenVpnGenerateCommand(this.userid, this.vpnseverid, 
                                             sshClient, sshRevokeClient, sftpClient);
        
                vpn.Execute();

            }

        }

    }
}

