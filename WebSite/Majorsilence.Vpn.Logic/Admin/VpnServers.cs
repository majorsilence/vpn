using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;

namespace LibLogic.Admin
{
    public class VpnServers
    {

        public IEnumerable<LibPoco.VpnServers> Select()
        {

            using (var db = Setup.DbFactory)
            {
                var vpnList = db.Query<LibPoco.VpnServers>("SELECT * FROM VpnServers");
                return vpnList;
            }

        }

        public LibPoco.VpnServers Select(int id)
        {

            using (var db = Setup.DbFactory)
            {
                return db.Get<LibPoco.VpnServers>(id);
            }

        }

        public int Insert(string address, int port, string description, 
                          int regionId, bool active)
        {
            using (var db = Setup.DbFactory)
            {
                db.Open();

                var vpn = new LibPoco.VpnServers(address,
                              port, description,
                              regionId, active);

                return (int)db.Insert(vpn);

            }
        }

        public void Update(int id, string address, int vpnPort, string description,
                           int regionId, bool active)
        {
            using (var db = LibLogic.Setup.DbFactory)
            {
              
                // update existing
                var vpn = db.Get<LibPoco.VpnServers>(id);
                vpn.Address = address;
                vpn.Description = description;
                vpn.VpnPort = vpnPort;
                vpn.RegionId = regionId;
                vpn.Active = active;

                db.Update(vpn);

            }
        }

    }
}
