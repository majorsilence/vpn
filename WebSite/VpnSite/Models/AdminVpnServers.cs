using System;
using System.Collections.Generic;

namespace VpnSite.Models
{
    public class AdminVpnServers
    {
        public AdminVpnServers()
        {
            _vpnServersList = new LibLogic.Admin.VpnServers().Select();
        }

        private readonly IEnumerable<LibPoco.VpnServers> _vpnServersList;
        public  IEnumerable<LibPoco.VpnServers> VpnServersList
        {
            get
            {
                return _vpnServersList;
            }
        }
    }
}

