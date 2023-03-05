using System;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Logic;

public class ActionLog
{
    private readonly DatabaseSettings _dbSettings;
    public ActionLog(DatabaseSettings dbSettings)
    {
        _dbSettings = dbSettings;
    }
    
    public void Log(string action, int userid)
    {
        using (var cn = _dbSettings.DbFactory)
        {
            cn.Open();

            var data = new Poco.ActionLog
            {
                Action = action,
                ActionDate = DateTime.UtcNow,
                UserId = userid
            };

            cn.Insert(data);
        }
    }
}