using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiteTests.Tests
{
    public class PPTPServerTest
    {
        public void CreatePptp()
        {
            using (var sshNewServer = new LibLogic.Ssh.FakeSsh())
            using (var sshRevokeServer = new LibLogic.Ssh.FakeSsh())
            {
                var ppt = new LibLogic.Ppp.ManagePPTP(2, 1, sshNewServer, sshRevokeServer);
                ppt.AddUser();
            }

        }
    }
}
