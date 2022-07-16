using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Majorsilence.Vpn.Poco
{
    [Dapper.Contrib.Extensions.Table("DatabaseInfo")]
    public class DatabaseInfo
    {
        public DatabaseInfo() { }
        public DatabaseInfo(string versionId, DateTime createTime, DateTime lastDailyProcess) 
        {
            this.VersionId = versionId;
            this.CreateTime = createTime;
            this.LastDailyProcess = lastDailyProcess;
        }

        [Dapper.Contrib.Extensions.Key()]
        public int Id { get; set; }

        public string VersionId { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastDailyProcess { get; set; }

    }
}