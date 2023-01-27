using System;
using System.Collections.Generic;

namespace Majorsilence.Vpn.Site.Models;

public class AdminVpnServers : CustomViewLayout
{
    public AdminVpnServers()
    {
        _vpnServersList = new Logic.Admin.VpnServers().Select();
    }

    private readonly IEnumerable<Poco.VpnServers> _vpnServersList;
    public IEnumerable<Poco.VpnServers> VpnServersList => _vpnServersList;
}