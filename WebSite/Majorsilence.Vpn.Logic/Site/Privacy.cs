using System.Linq;
using Dapper;

namespace Majorsilence.Vpn.Logic.Site;

public class Privacy
{
    private readonly DatabaseSettings _dbSettings;
    public Privacy(DatabaseSettings dbSettings)
    {
        _dbSettings = dbSettings;
    }
    
    public Poco.Privacy CurrentPrivacy()
    {
        using (var cn = _dbSettings.DbFactory)
        {
            cn.Open();

            var data = cn.Query<Poco.Privacy>(
                "SELECT * FROM Privacy ORDER BY CreateTime DESC Limit 1");

            return data.First();
        }
    }
}