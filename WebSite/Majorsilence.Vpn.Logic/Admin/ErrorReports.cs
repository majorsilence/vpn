using System.Collections.Generic;
using Dapper;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Logic.Admin;

public class ErrorReports
{
    public IEnumerable<Errors> RetrieveAll()
    {
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            return db.Query<Errors>("SELECT * FROM Errors ORDER BY Id desc");
        }
    }

    public IEnumerable<Errors> RetrieveLimit(int start, int count)
    {
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            return db.Query<Errors>("SELECT * FROM Errors LIMIT @Start, @Count",
                new { Start = start, Count = count });
        }
    }
}