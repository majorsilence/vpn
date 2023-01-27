using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Logic.Helpers;

public class Logging
{
    public static void Log(string msg, bool emailErrorToAdmin)
    {
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            db.Insert<Poco.Errors>(new Poco.Errors(
                DateTime.UtcNow,
                msg,
                "",
                "")
            );
        }

        if (emailErrorToAdmin)
            InitializeSettings.Email.SendMail_BackgroundThread("Message: " + msg + Environment.NewLine,
                "Msg somewhere in the system", SiteInfo.AdminEmail, true, null,
                Email.EmailTemplates.Generic);
    }

    public static void Log(Exception ex, bool emailErrorToAdmin)
    {
        var innerException = GetInnerException(ex);
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            db.Insert<Poco.Errors>(new Poco.Errors(
                DateTime.UtcNow,
                ex.Message,
                ex.StackTrace == null ? "" : ex.StackTrace.SafeSubstring(0, 4000),
                innerException == null ? "" : innerException.SafeSubstring(0, 8000))
            );
        }

        if (emailErrorToAdmin)
            InitializeSettings.Email.SendMail_BackgroundThread(
                "Error: " + ex.Message + Environment.NewLine + innerException,
                "Error somewhere in the system", SiteInfo.AdminEmail, true, null,
                Email.EmailTemplates.Generic);
    }

    public static void Log(Exception ex)
    {
        Log(ex, false);
    }

    private static string GetInnerException(Exception ex)
    {
        var msg = string.Empty;


        if (null != ex)
        {
            msg = ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace +
                  Environment.NewLine + Environment.NewLine;

            if (null != ex.InnerException) msg = msg + GetInnerException(ex.InnerException);
        }

        return msg;
    }
}