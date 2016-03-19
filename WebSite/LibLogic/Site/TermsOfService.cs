using System;
using System.Linq;
using Dapper;

namespace LibLogic.Site
{
    public class TermsOfService
    {
        public TermsOfService()
        {
        }

        public LibPoco.TermsOfService CurrentTermsOfService()
        {
            using (var cn = Setup.DbFactory)
            {
                cn.Open();

                var data = cn.Query<LibPoco.TermsOfService>("SELECT * FROM TermsOfService ORDER BY CreateTime DESC Limit 1");

                return data.First();
            }

        }

    }
}

