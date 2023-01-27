using System;
using System.Collections.Generic;
using Dapper;

namespace Majorsilence.Vpn.Logic.Admin;

public class ErrorReports
{
    public ErrorReports()
    {
    }

    public IEnumerable<Poco.Errors> RetrieveAll()
    {
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            return db.Query<Poco.Errors>("SELECT * FROM Errors ORDER BY Id desc");
        }
    }

    public IEnumerable<Poco.Errors> RetrieveLimit(int start, int count)
    {
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            return db.Query<Poco.Errors>("SELECT * FROM Errors LIMIT @Start, @Count",
                new { Start = start, Count = count });
        }
    }
}