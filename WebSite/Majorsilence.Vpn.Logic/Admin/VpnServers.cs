using System.Collections.Generic;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Logic.Admin;

public class VpnServers
{
    private readonly DatabaseSettings _dbSettings;
    public VpnServers(DatabaseSettings dbSettings)
    {
        _dbSettings = dbSettings;
    }
    
    public IEnumerable<Poco.VpnServers> Select()
    {
        using (var db = _dbSettings.DbFactory)
        {
            var vpnList = db.Query<Poco.VpnServers>("SELECT * FROM VpnServers");
            return vpnList;
        }
    }

    public Poco.VpnServers Select(int id)
    {
        using (var db = _dbSettings.DbFactory)
        {
            return db.Get<Poco.VpnServers>(id);
        }
    }

    public int Insert(string address, int port, string description,
        int regionId, bool active)
    {
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();

            var vpn = new Poco.VpnServers(address,
                port, description,
                regionId, active);

            return (int)db.Insert(vpn);
        }
    }

    public void Update(int id, string address, int vpnPort, string description,
        int regionId, bool active)
    {
        using (var db = _dbSettings.DbFactory)
        {
            // update existing
            var vpn = db.Get<Poco.VpnServers>(id);
            vpn.Address = address;
            vpn.Description = description;
            vpn.VpnPort = vpnPort;
            vpn.RegionId = regionId;
            vpn.Active = active;

            db.Update(vpn);
        }
    }
}