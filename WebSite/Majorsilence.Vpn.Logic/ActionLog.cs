using System;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Logic
{
    public class ActionLog
    {
        public ActionLog()
        {
        }

        public void Log(string action, int userid)
        {
            using (var cn = Majorsilence.Vpn.Logic.InitializeSettings.DbFactory)
            {
                cn.Open();

                var data = new Majorsilence.Vpn.Poco.ActionLog()
                {
                    Action = action,
                    ActionDate = DateTime.UtcNow,
                    UserId = userid
                };

                cn.Insert(data);

            }

        }

        public static void Log_BackgroundThread(string action, int userid)
        {
        
            Task.Run(() =>
            {
                var act = new ActionLog();
                act.Log(action, userid);
            });

        }

    }
}

