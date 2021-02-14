using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Logic.Helpers
{
    public class Logging
    {
    

        public static void Log(string msg, bool emailErrorToAdmin)
        {
        
            using (IDbConnection db = Setup.DbFactory)
            {
                db.Open();
                db.Insert<Majorsilence.Vpn.Poco.Errors>(new Majorsilence.Vpn.Poco.Errors(
                    DateTime.UtcNow,
                    msg,
                    "",
                    "")
                );

            }

            if (emailErrorToAdmin)
            {

                Setup.Email.SendMail_BackgroundThread("Message: " + msg + System.Environment.NewLine,
                    "Msg somewhere in the system", Helpers.SiteInfo.AdminEmail, true, null,
                    Email.EmailTemplates.Generic);
            }

        }

        public static void Log(Exception ex, bool emailErrorToAdmin)
        {
			
            string innerException = GetInnerException(ex);
            using (var db = Setup.DbFactory)
            {
                db.Open();
                db.Insert<Majorsilence.Vpn.Poco.Errors>(new Majorsilence.Vpn.Poco.Errors(
                    DateTime.UtcNow,
                    ex.Message,
                    ex.StackTrace == null ? "" : ex.StackTrace.SafeSubstring(0, 4000),
                    innerException == null ? "" : innerException.SafeSubstring(0, 8000))
                );

            }

            if (emailErrorToAdmin)
            {

                Setup.Email.SendMail_BackgroundThread("Error: " + ex.Message + System.Environment.NewLine + innerException,
                    "Error somewhere in the system", Helpers.SiteInfo.AdminEmail, true, null,
                    Email.EmailTemplates.Generic);
            }

        }

        public static void Log(Exception ex)
        {

            Log(ex, false);

        }

        private static string GetInnerException(Exception ex)
        {

            string msg = string.Empty;


            if ((null != ex))
            {
                msg = ex.Message + System.Environment.NewLine + ex.Source + System.Environment.NewLine + ex.StackTrace + System.Environment.NewLine + System.Environment.NewLine;

                if ((null != ex.InnerException))
                {
                    msg = msg + GetInnerException(ex.InnerException);
                }

            }

            return msg;

        }
    }
}