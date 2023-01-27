using System.Collections.Generic;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Logic.Admin;

public class Regions
{
    public IEnumerable<Poco.Regions> Select()
    {
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            var regions = db.Query<Poco.Regions>("SELECT * FROM Regions");
            return regions;
        }
    }

    public int Insert(string description, bool active)
    {
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            var region = new Poco.Regions(description, active);
            return (int)db.Insert(region);
        }
    }

    public void Update(int id, string description, bool active)
    {
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            // update existing
            var region = db.Get<Poco.Regions>(id);
            region.Description = description;
            region.Active = active;
            db.Update(region);
        }
    }
}