using System;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;

namespace LibLogic
{
    public class ActionLog
    {
        public ActionLog()
        {
        }

        public void Log(string action, int userid)
        {
            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();

                var data = new LibPoco.ActionLog()
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

