using System;
using System.Linq;
using Dapper;

namespace Majorsilence.Vpn.Logic.Site
{
    public class Privacy
    {
        public Privacy()
        {
        }

        public Majorsilence.Vpn.Poco.Privacy CurrentPrivacy()
        {
            using (var cn = InitializeSettings.DbFactory)
            {
                cn.Open();

                var data = cn.Query<Majorsilence.Vpn.Poco.Privacy>("SELECT * FROM Privacy ORDER BY CreateTime DESC Limit 1");

                return data.First();
            }

        }
    }
}

