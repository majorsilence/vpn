using System.Collections.Generic;
using System.Text;
using Dapper;

namespace Majorsilence.Vpn.Logic.Accounts;

/// <summary>
///     Retrieve active server details including region
/// </summary>
public class ServerDetails
{
    public ServerDetails(DatabaseSettings dbSettings)
    {
        //  this._userId = userId;

        var sql = new StringBuilder();
        sql.Append(
            "select a.Id as 'VpnServerId', b.Id as 'RegionId', a.Description as 'VpnServerName', b.Description as 'RegionName', a.Address ");
        sql.Append("from VpnServers a ");
        sql.Append("join Regions b on a.RegionId = b.Id WHERE a.Active=1");

        using (var db = dbSettings.DbFactory)
        {
            db.Open();
            Info = db.Query<UserServerDetailsInfo>(sql.ToString());
        }
    }

    public IEnumerable<UserServerDetailsInfo> Info { get; }
}