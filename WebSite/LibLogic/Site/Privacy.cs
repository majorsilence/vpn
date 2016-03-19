using System;
using System.Linq;
using Dapper;

namespace LibLogic.Site
{
    public class Privacy
    {
        public Privacy()
        {
        }

        public LibPoco.Privacy CurrentPrivacy()
        {
            using (var cn = Setup.DbFactory)
            {
                cn.Open();

                var data = cn.Query<LibPoco.Privacy>("SELECT * FROM Privacy ORDER BY CreateTime DESC Limit 1");

                return data.First();
            }

        }
    }
}

