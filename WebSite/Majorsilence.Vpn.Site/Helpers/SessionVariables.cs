using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Majorsilence.Vpn.Site.Helpers
{
    public class SessionVariables : ISessionVariables
    {
        HttpContext context;
        public SessionVariables(IHttpContextAccessor context)
        {
            this.context = context.HttpContext;
        }

        //  public static SessionVariables Instance = new SessionVariables();

        public bool LoggedIn
        {
            get
            {


                if (context.Session.GetString("LoggedIn") == null)
                {
                    return false;
                }
                return Convert.ToBoolean(context.Session.GetString("LoggedIn"));
            }
            set
            {
                context.Session.SetString("LoggedIn", value.ToString());
            }
        }


        public string Username
        {
            get
            {
                if (context.Session == null)
                {
                    return "";
                }
                if (context.Session.GetString("Username") == null)
                {
                    return "";
                }
                return context.Session.GetString("Username");
            }
            set
            {
                context.Session.SetString("Username", value);
            }
        }

        public int UserId
        {
            get
            {
                if (context.Session.GetString("UserId") == null)
                {
                    return -1;
                }
                return Convert.ToInt32(context.Session.GetString("UserId"));
            }
            set
            {
                context.Session.SetString("UserId", value.ToString());
            }
        }

        /// <summary>
        /// Check if the logged in user is an admin
        /// </summary>
        public bool IsAdmin
        {
            get
            {
                if (context.Session.GetString("IsAdmin") == null)
                {
                    return false;
                }
                return Convert.ToBoolean(context.Session.GetString("IsAdmin"));
            }
            set
            {
                context.Session.SetString("IsAdmin", value.ToString());
            }
        }

    }

}