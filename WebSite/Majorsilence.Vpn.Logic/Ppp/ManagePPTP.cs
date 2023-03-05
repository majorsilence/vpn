using Majorsilence.Vpn.Logic.Ssh;

namespace Majorsilence.Vpn.Logic.Ppp;

public class ManagePPTP : PppBase
{
    public ManagePPTP(int userId, int vpnServerId, ISsh sshNewServer, ISsh sshRevokeServer,
        DatabaseSettings dbSettings)
        : base(userId, vpnServerId, sshNewServer, sshRevokeServer, dbSettings)
    {
    }

    protected override void AddUserImplementation(ISsh sshClient)
    {
        // TODO: will need an unhashed user password
        sshClient.WriteLine(string.Format("echo \"{0} {1} {2} *\" >> /etc/ppp/chap-secrets",
            userData.Email, "pptpd", userRequestedPassword));
    }

    protected override void RevokeUserImplementation(ISsh sshClient)
    {
        sshClient.WriteLine(string.Format("sed -i '/^{0} pptpd/d' /etc/ppp/chap-secrets", userData.Email));
    }
}