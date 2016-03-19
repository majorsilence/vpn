using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.Routing;

namespace VpnSite.views.resetpassword
{
    /// <summary>
    /// 
    /// </summary>
    public class ResetPasswordSend : Helpers.IHttpHandlerBase, IRequiresSessionState
    {

        public RequestContext RequestContext { get; set; }  

        public void ProcessRequest(HttpContext context)
        {

			try
			{
                var email = new LibLogic.Email.LiveEmail();
	            string username = Helpers.GlobalHelper.RequestEncodedParam("username");
	            var resetPSW = new LibLogic.Accounts.ResetPassword(email);
	            resetPSW.sendPasswordLink(username);
			}
            catch(LibLogic.Exceptions.InvalidDataException ide) {
				context.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
				context.Response.Write(ide.Message);
			}
			catch(Exception ex) {
				context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
				context.Response.Write(ex.Message);
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