using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;

namespace Majorsilence.Vpn.Logic.Admin;

public class Regions
{
    public IEnumerable<Majorsilence.Vpn.Poco.Regions> Select()
    {
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            var regions = db.Query<Majorsilence.Vpn.Poco.Regions>("SELECT * FROM Regions");
            return regions;
        }
    }

    public int Insert(string description, bool active)
    {
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            var region = new Majorsilence.Vpn.Poco.Regions(description, active);
            return (int)db.Insert(region);
        }
    }

    public void Update(int id, string description, bool active)
    {
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            // update existing
            var region = db.Get<Majorsilence.Vpn.Poco.Regions>(id);
            region.Description = description;
            region.Active = active;
            db.Update(region);
        }
    }
}