using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.Routing;

namespace VpnSite.views.resetpassword
{
    public class ResetCodeValidation : Helpers.IHttpHandlerBase, IRequiresSessionState
    {
        public RequestContext RequestContext { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            string resetCode = Helpers.GlobalHelper.RequestEncodedParam("code");
            string cnewpsw = Helpers.GlobalHelper.RequestEncodedParam("cnewpsw");
            string newpsw = Helpers.GlobalHelper.RequestEncodedParam("newpsw");

            if (!string.IsNullOrEmpty(resetCode) && !string.IsNullOrEmpty(cnewpsw) && !string.IsNullOrEmpty(newpsw))
            {
                if (cnewpsw != newpsw)
                {
                    throw new HttpException(400, "Password and Confirm Password should match");
                }
                var email = new LibLogic.Email.LiveEmail();
                var resetPSW = new LibLogic.Accounts.ResetPassword(email);
                resetPSW.validateCode(resetCode, newpsw);
                context.Response.StatusCode = 250;
            }
            else {
                throw new HttpException(400, "Invalid Data");
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