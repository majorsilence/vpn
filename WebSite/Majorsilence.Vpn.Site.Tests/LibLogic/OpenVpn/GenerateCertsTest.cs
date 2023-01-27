using Majorsilence.Vpn.Logic.OpenVpn;
using Majorsilence.Vpn.Logic.Ssh;
using NUnit.Framework;

namespace SiteTests.Tests;

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
    ///     Test creating a cert and downloading from vpn server to web site
    /// </summary>
    [Test]
    public void CreateCert()
    {
        using (var sshNewServer = new FakeSsh())
        using (var sshRevokeServer = new FakeSsh())
        using (var sftp = new FakeSftp())
        {
            var certs = new CertsOpenVpnGenerateCommand(2, 1, sshNewServer,
                sshRevokeServer, sftp);
            certs.Execute();
        }
    }
}