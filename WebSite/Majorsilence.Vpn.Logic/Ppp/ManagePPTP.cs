using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace LibLogic.Ppp
{
    public class ManagePPTP : PppBase
    {

        public ManagePPTP(int userId, int vpnServerId, Ssh.ISsh sshNewServer, Ssh.ISsh sshRevokeServer)
            : base(userId, vpnServerId, sshNewServer, sshRevokeServer)
        {
        }

        protected override void AddUserImplementation(LibLogic.Ssh.ISsh sshClient)
        {
            // TODO: will need an unhashed user password
            sshClient.WriteLine(string.Format("echo \"{0} {1} {2} *\" >> /etc/ppp/chap-secrets", 
                userData.Email, "pptpd", this.userRequestedPassword));
        }

        protected override void RevokeUserImplementation(LibLogic.Ssh.ISsh sshClient)
        {
            sshClient.WriteLine(string.Format("sed -i '/^{0} pptpd/d' /etc/ppp/chap-secrets", userData.Email));
        }
    }
}