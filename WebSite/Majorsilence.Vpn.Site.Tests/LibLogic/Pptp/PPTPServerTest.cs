using Majorsilence.Vpn.Logic.Ppp;
using Majorsilence.Vpn.Logic.Ssh;

namespace SiteTests.Tests;

public class PPTPServerTest
{
    public void CreatePptp()
    {
        using (var sshNewServer = new FakeSsh())
        using (var sshRevokeServer = new FakeSsh())
        {
            var ppt = new ManagePPTP(2, 1, sshNewServer, sshRevokeServer);
            ppt.AddUser();
        }
    }
}