using System;
using System.Linq;
using Dapper;

namespace Majorsilence.Vpn.Logic.Site
{
    public class TermsOfService
    {
        public TermsOfService()
        {
        }

        public Majorsilence.Vpn.Poco.TermsOfService CurrentTermsOfService()
        {
            using (var cn = InitializeSettings.DbFactory)
            {
                cn.Open();

                var data = cn.Query<Majorsilence.Vpn.Poco.TermsOfService>("SELECT * FROM TermsOfService ORDER BY CreateTime DESC Limit 1");

                return data.First();
            }

        }

    }
}

