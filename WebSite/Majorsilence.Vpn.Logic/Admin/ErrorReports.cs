using System;
using System.Collections.Generic;
using Dapper;

namespace Majorsilence.Vpn.Logic.Admin
{
    public class ErrorReports
    {
        public ErrorReports()
        {
        }

        public IEnumerable<Majorsilence.Vpn.Poco.Errors> RetrieveAll()
        {
            using (var db = InitializeSettings.DbFactory)
            {
                db.Open();
                return db.Query<Majorsilence.Vpn.Poco.Errors>("SELECT * FROM Errors ORDER BY Id desc");

            }
        }

        public IEnumerable<Majorsilence.Vpn.Poco.Errors> RetrieveLimit(int start, int count)
        {
            using (var db = InitializeSettings.DbFactory)
            {
                db.Open();
                return db.Query<Majorsilence.Vpn.Poco.Errors>("SELECT * FROM Errors LIMIT @Start, @Count", new {Start = start, Count = count});

            }

        }
    }
}

