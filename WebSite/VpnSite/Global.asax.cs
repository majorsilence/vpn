using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Http;

namespace VpnSite
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterWebFormRoutes(RouteTable.Routes);
            RegisterRazorRoutes(RouteTable.Routes);

            var mySqlInstance = System.Configuration.ConfigurationManager.AppSettings["MySqlInstance"].ToString();
            var mySqlDatabase = System.Configuration.ConfigurationManager.AppSettings["MySqlDatabase"].ToString();
            var email = new LibLogic.Email.LiveEmail();
            var setup = new LibLogic.Setup(mySqlInstance, mySqlDatabase, email, false);

            try
            {
                setup.Execute();
            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                email.SendMail_BackgroundThread("It appears the server setup failed: " + ex.Message,
                    "MajorsilnceVPN setup failure on application_start", LibLogic.Helpers.SiteInfo.AdminEmail, false, null);
            }
        }


        void RegisterRazorRoutes(RouteCollection routes)
        {
            // missing in mono
            // routes.LowercaseUrls = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default",
                "{controller}/{action}/{id}", 
                new { controller = "Home", action = "Index", id = "" });
                         

        }

        void RegisterWebFormRoutes(RouteCollection routes)
        {
            				
            routes.Add(new Route("downloadopenvpncert", new Helpers.GenericRouteHandler<views.setup.downopenvpncert>()));
            routes.Add(new Route("saveuservpnserver", new Helpers.GenericRouteHandler<views.setup.saveuservpnserver>()));
            routes.Add(new Route("loginvalidation", new Helpers.GenericRouteHandler<views.login.loginvalidation>()));
            routes.Add(new Route("resetpasswordsend", new Helpers.GenericRouteHandler<views.resetpassword.ResetPasswordSend>()));
            routes.Add(new Route("resetcodevalidation", new Helpers.GenericRouteHandler<views.resetpassword.ResetCodeValidation>()));
            routes.Add(new Route("stripewebhook", new Helpers.GenericRouteHandler<views.account.stripe.stripewebhook>()));

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}