using System;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Poco;

[Table("DatabaseInfo")]
public class DatabaseInfo
{
    public DatabaseInfo()
    {
    }

    public DatabaseInfo(string versionId, DateTime createTime, DateTime lastDailyProcess)
    {
        VersionId = versionId;
        CreateTime = createTime;
        LastDailyProcess = lastDailyProcess;
    }

    [Key] public int Id { get; set; }

    public string VersionId { get; set; }

    public DateTime CreateTime { get; set; }

    public DateTime LastDailyProcess { get; set; }
}