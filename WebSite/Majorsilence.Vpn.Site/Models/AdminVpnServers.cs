using System.Collections.Generic;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Site.Models;

public class AdminVpnServers : CustomViewLayout
{
    public AdminVpnServers(DatabaseSettings dbSettings)
    {
        VpnServersList = new Logic.Admin.VpnServers(dbSettings).Select();
    }

    public IEnumerable<VpnServers> VpnServersList { get; }
}