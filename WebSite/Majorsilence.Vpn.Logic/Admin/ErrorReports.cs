using System.Collections.Generic;
using Dapper;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Logic.Admin;

public class ErrorReports
{
    private readonly DatabaseSettings _dbSettings;
    public ErrorReports(DatabaseSettings dbSettings)
    {
        _dbSettings = dbSettings;
    }
    
    public IEnumerable<Errors> RetrieveAll()
    {
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();
            return db.Query<Errors>("SELECT * FROM Errors ORDER BY Id desc");
        }
    }

    public IEnumerable<Errors> RetrieveLimit(int start, int count)
    {
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();
            return db.Query<Errors>("SELECT * FROM Errors LIMIT @Start, @Count",
                new { Start = start, Count = count });
        }
    }
}