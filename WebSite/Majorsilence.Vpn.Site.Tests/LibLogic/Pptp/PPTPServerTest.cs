using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiteTests.Tests;

public class PPTPServerTest
{
    public void CreatePptp()
    {
        using (var sshNewServer = new Majorsilence.Vpn.Logic.Ssh.FakeSsh())
        using (var sshRevokeServer = new Majorsilence.Vpn.Logic.Ssh.FakeSsh())
        {
            var ppt = new Majorsilence.Vpn.Logic.Ppp.ManagePPTP(2, 1, sshNewServer, sshRevokeServer);
            ppt.AddUser();
        }
    }
}