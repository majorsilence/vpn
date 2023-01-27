using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Logic.Accounts;

public class UserServerDetails
{
    private UserServerDetails()
    {
    }

    private UserServerDetailsInfo details = null;

    public UserServerDetails(int userId)
    {
        //  this._userId = userId;

        var sql = new System.Text.StringBuilder();
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
                details = info.FirstOrDefault();
            else if (info.Count() > 1)
                throw new Exceptions.InvalidDataException(
                    "Some how you have multiple vpn certs and that is not supposed to be possible yet.");


            var pptp = db.Query<Poco.UserPptpInfo>("SELECT * FROM UserPptpInfo WHERE UserId=@UserId",
                new { UserId = userId });
            if (pptp.Count() == 1)
                details.PptpPassword = pptp.FirstOrDefault().Password;
            else if (pptp.Count() > 1)
                throw new Exceptions.InvalidDataException(
                    "Some how you have multiple pptp accounts or vpn certs and that is not supposed to be possible yet.");
        }
    }

    public UserServerDetailsInfo Info => details;
}