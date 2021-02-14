using System;
using System.Collections.Generic;
using Dapper;

namespace LibLogic.Admin
{
    public class ErrorReports
    {
        public ErrorReports()
        {
        }

        public IEnumerable<LibPoco.Errors> RetrieveAll()
        {
            using (var db = Setup.DbFactory)
            {
                db.Open();
                return db.Query<LibPoco.Errors>("SELECT * FROM Errors ORDER BY Id desc");

            }
        }

        public IEnumerable<LibPoco.Errors> RetrieveLimit(int start, int count)
        {
            using (var db = Setup.DbFactory)
            {
                db.Open();
                return db.Query<LibPoco.Errors>("SELECT * FROM Errors LIMIT @Start, @Count", new {Start = start, Count = count});

            }

        }
    }
}

