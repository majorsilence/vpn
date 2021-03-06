using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Logic.Accounts
{
    /// <summary>
    /// Retrieve active server details including region
    /// </summary>
    public class ServerDetails
    {
        private IEnumerable<UserServerDetailsInfo> details = null;

        public ServerDetails()
        {
            //  this._userId = userId;

            var sql = new System.Text.StringBuilder();
            sql.Append("select a.Id as 'VpnServerId', b.Id as 'RegionId', a.Description as 'VpnServerName', b.Description as 'RegionName', a.Address ");
            sql.Append("from VpnServers a ");
            sql.Append("join Regions b on a.RegionId = b.Id WHERE a.Active=1");

            using (var db = InitializeSettings.DbFactory)
            {
                db.Open();
                details = db.Query<UserServerDetailsInfo>(sql.ToString());
            }


        }

        public IEnumerable<UserServerDetailsInfo> Info
        {
            get
            {
                return details;
            }
        }
    }
}