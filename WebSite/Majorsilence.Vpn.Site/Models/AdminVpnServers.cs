using System;
using System.Collections.Generic;

namespace Majorsilence.Vpn.Site.Models
{
    public class AdminVpnServers : CustomViewLayout
    {
        public AdminVpnServers()
        {
            _vpnServersList = new Majorsilence.Vpn.Logic.Admin.VpnServers().Select();
        }

        private readonly IEnumerable<Majorsilence.Vpn.Poco.VpnServers> _vpnServersList;
        public  IEnumerable<Majorsilence.Vpn.Poco.VpnServers> VpnServersList
        {
            get
            {
                return _vpnServersList;
            }
        }
    }
}

