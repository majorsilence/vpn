using System.Collections.Generic;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Site.Models;

public class AdminVpnServers : CustomViewLayout
{
    public AdminVpnServers()
    {
        VpnServersList = new Logic.Admin.VpnServers().Select();
    }

    public IEnumerable<VpnServers> VpnServersList { get; }
}