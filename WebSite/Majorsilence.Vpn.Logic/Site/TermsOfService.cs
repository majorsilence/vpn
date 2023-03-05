using System.Linq;
using Dapper;

namespace Majorsilence.Vpn.Logic.Site;

public class TermsOfService
{
    private readonly DatabaseSettings _dbSettings;
    public TermsOfService(DatabaseSettings dbSettings)
    {
        _dbSettings = dbSettings;
    }
    public Poco.TermsOfService CurrentTermsOfService()
    {
        using (var cn = _dbSettings.DbFactory)
        {
            cn.Open();

            var data = cn.Query<Poco.TermsOfService>(
                "SELECT * FROM TermsOfService ORDER BY CreateTime DESC Limit 1");

            return data.First();
        }
    }
}