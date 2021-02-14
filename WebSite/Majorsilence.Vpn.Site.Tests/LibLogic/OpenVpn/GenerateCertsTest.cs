using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SiteTests.Tests
{
    public class GenerateCertsTest
    {
        [SetUp]
        public void Setup()
        {
            // Create vpn account user(s) required for below tests
        }

        [TearDown]
        public void TearDown()
        {
            // Delete vpn account users(s) for below tests
        }

        /// <summary>
        /// Test creating a cert and downloading from vpn server to web site
        /// </summary>
        [Test()]
        public void CreateCert()
        {
            using (var sshNewServer = new Majorsilence.Vpn.Logic.Ssh.FakeSsh())
            using (var sshRevokeServer = new Majorsilence.Vpn.Logic.Ssh.FakeSsh())
            using (var sftp = new Majorsilence.Vpn.Logic.Ssh.FakeSftp())
            {
                var certs = new Majorsilence.Vpn.Logic.OpenVpn.CertsOpenVpnGenerateCommand(2, 1, sshNewServer, sshRevokeServer, sftp);
                certs.Execute();

            }

        }

    }
}
