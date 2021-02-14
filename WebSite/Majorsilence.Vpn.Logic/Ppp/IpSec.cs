using System;
using System.IO;

namespace Majorsilence.Vpn.Logic.Ppp
{
    public class IpSec : PppBase
    {
        public IpSec(int userId, int vpnServerId, Ssh.ISsh sshNewServer, Ssh.ISsh sshRevokeServer)
            : base(userId, vpnServerId, sshNewServer, sshRevokeServer)
        {
        }

        protected override void AddUserImplementation(Majorsilence.Vpn.Logic.Ssh.ISsh sshClient)
        {


            // TODO: will need an unhashed user password
            sshClient.WriteLine(string.Format("echo \"{0} {1} {2} *\" >> /etc/ppp/chap-secrets", 
                userData.Email, "l2tpd", this.userRequestedPassword));


        }

        protected override void RevokeUserImplementation(Majorsilence.Vpn.Logic.Ssh.ISsh sshClient)
        {
            sshClient.WriteLine(string.Format("sed -i '/^{0} l2tpd/d' /etc/ppp/chap-secrets", userData.Email));
        }
    }
}

