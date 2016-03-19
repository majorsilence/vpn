using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.Routing;

namespace VpnSite.views.account.stripe
{
    /// <summary>
    /// Summary description for stripewebhook
    /// </summary>
	public class stripewebhook : Helpers.IHttpHandlerBase, IRequiresSessionState
    {
		public RequestContext RequestContext { get; set; }  

        public void ProcessRequest(HttpContext context)
        {

            try
            {
                var json = new StreamReader(context.Request.InputStream).ReadToEnd();
                var hook = new LibLogic.Payments.StripWebHook(json);
                hook.Execute();
              
                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = 200;

            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex, true);
                context.Response.StatusCode = 500;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}