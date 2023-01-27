using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;

namespace Majorsilence.Vpn.Logic.Admin;

public class VpnServers
{
    public IEnumerable<Majorsilence.Vpn.Poco.VpnServers> Select()
    {
        using (var db = InitializeSettings.DbFactory)
        {
            var vpnList = db.Query<Majorsilence.Vpn.Poco.VpnServers>("SELECT * FROM VpnServers");
            return vpnList;
        }
    }

    public Majorsilence.Vpn.Poco.VpnServers Select(int id)
    {
        using (var db = InitializeSettings.DbFactory)
        {
            return db.Get<Majorsilence.Vpn.Poco.VpnServers>(id);
        }
    }

    public int Insert(string address, int port, string description,
        int regionId, bool active)
    {
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();

            var vpn = new Majorsilence.Vpn.Poco.VpnServers(address,
                port, description,
                regionId, active);

            return (int)db.Insert(vpn);
        }
    }

    public void Update(int id, string address, int vpnPort, string description,
        int regionId, bool active)
    {
        using (var db = InitializeSettings.DbFactory)
        {
            // update existing
            var vpn = db.Get<Majorsilence.Vpn.Poco.VpnServers>(id);
            vpn.Address = address;
            vpn.Description = description;
            vpn.VpnPort = vpnPort;
            vpn.RegionId = regionId;
            vpn.Active = active;

            db.Update(vpn);
        }
    }
}