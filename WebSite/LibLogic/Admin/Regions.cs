using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;

namespace LibLogic.Admin
{
    public class Regions
    {

        public IEnumerable<LibPoco.Regions> Select()
        {
            using (IDbConnection db = Setup.DbFactory)
            {
                db.Open();
                var regions = db.Query<LibPoco.Regions>("SELECT * FROM Regions");
                return regions;
            }


        }

        public int Insert(string description, bool active)
        {
        
            using (IDbConnection db = Setup.DbFactory)
            {
                db.Open();
                var region = new LibPoco.Regions(description, active);
                return (int)db.Insert(region);
             
            }
        }

        public void Update(int id, string description, bool active)
        {
        
            using (IDbConnection db = Setup.DbFactory)
            {
                db.Open();
                // update existing
                var region = db.Get<LibPoco.Regions>(id);
                region.Description = description;
                region.Active = active;
                db.Update(region);
               
                
            }

        }

    }
}
