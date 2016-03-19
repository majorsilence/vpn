using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibPoco
{
    [Dapper.Contrib.Extensions.Table("VpnServers")]
    public class VpnServers
    {
        public VpnServers() { }
        public VpnServers(string address, int vpnPort, string description, int regionId, bool active) 
        {
            this.Address = address;
            this.VpnPort = vpnPort;
            this.Description = description;
            this.RegionId = regionId;
            this.Active = active;
        }

        [Dapper.Contrib.Extensions.Key()]
        public int Id { get; set; }

        public string Address { get; set; }

        public int VpnPort { get; set; }

        public string Description { get; set; }

        public int RegionId { get; set; }

        public bool Active { get; set; }

    }
}