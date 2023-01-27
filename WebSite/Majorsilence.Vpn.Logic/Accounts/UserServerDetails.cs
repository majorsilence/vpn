using System.Linq;
using System.Text;
using Dapper;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Logic.Accounts;

public class UserServerDetails
{
    private UserServerDetails()
    {
    }

    public UserServerDetails(int userId)
    {
        //  this._userId = userId;

        var sql = new StringBuilder();
        sql.Append(
            "select a.Id as 'VpnServerId', b.Id as 'RegionId', a.Description as 'VpnServerName', b.Description as 'RegionName', a.Address, '' as 'Password'");
        sql.Append("from VpnServers a ");
        sql.Append("join Regions b on a.RegionId = b.Id ");
        sql.Append("join UserOpenVpnCerts c on c.VpnServersId = a.Id ");
        sql.Append("where c.userid = @uid");

        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            var info = db.Query<UserServerDetailsInfo>(sql.ToString(), new { uid = userId });
            if (info.Count() == 1)
                Info = info.FirstOrDefault();
            else if (info.Count() > 1)
                throw new InvalidDataException(
                    "Some how you have multiple vpn certs and that is not supposed to be possible yet.");


            var pptp = db.Query<UserPptpInfo>("SELECT * FROM UserPptpInfo WHERE UserId=@UserId",
                new { UserId = userId });
            if (pptp.Count() == 1)
                Info.PptpPassword = pptp.FirstOrDefault().Password;
            else if (pptp.Count() > 1)
                throw new InvalidDataException(
                    "Some how you have multiple pptp accounts or vpn certs and that is not supposed to be possible yet.");
        }
    }

    public UserServerDetailsInfo Info { get; }
}