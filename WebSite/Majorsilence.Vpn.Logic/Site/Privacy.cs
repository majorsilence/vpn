using System.Linq;
using Dapper;

namespace Majorsilence.Vpn.Logic.Site;

public class Privacy
{
    public Poco.Privacy CurrentPrivacy()
    {
        using (var cn = InitializeSettings.DbFactory)
        {
            cn.Open();

            var data = cn.Query<Poco.Privacy>(
                "SELECT * FROM Privacy ORDER BY CreateTime DESC Limit 1");

            return data.First();
        }
    }
}