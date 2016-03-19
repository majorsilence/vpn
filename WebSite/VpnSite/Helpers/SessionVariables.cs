using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VpnSite.Helpers
{
    public class SessionVariables : ISessionVariables
    {
        public static SessionVariables Instance = new SessionVariables();

        public bool LoggedIn
        {
            get
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                if (context.Session["LoggedIn"] == null)
                {
                    return false;
                }
                return Convert.ToBoolean(context.Session["LoggedIn"]);
            }
            set
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                context.Session["LoggedIn"] = value;
            }
        }


        public  string Username
        {
            get
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                if (context.Session == null)
                {
                    return "";
                }
                if (context.Session["Username"] == null)
                {
                    return "";
                }
                return context.Session["Username"].ToString();
            }
            set
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                context.Session["Username"] = value;
            }
        }

        public  int UserId
        {
            get
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                if (context.Session["UserId"] == null)
                {
                    return -1;
                }
                return Convert.ToInt32(context.Session["UserId"].ToString());
            }
            set
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                context.Session["UserId"] = value;
            }
        }

        /// <summary>
        /// Check if the logged in user is an admin
        /// </summary>
        public  bool IsAdmin
        {
            get
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                if (context.Session["IsAdmin"] == null)
                {
                    return false;
                }
                return Convert.ToBoolean(context.Session["IsAdmin"].ToString());
            }
            set
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                context.Session["IsAdmin"] = value;
            }
        }

    }

}