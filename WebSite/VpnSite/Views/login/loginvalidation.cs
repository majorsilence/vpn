using System.Web;
using System.Web.SessionState;
using System.Web.Routing;

namespace VpnSite.views.login
{
    /// <summary>
    /// Summary description for login
    /// </summary>
    public class loginvalidation : Helpers.IHttpHandlerBase, IRequiresSessionState
    {
        public RequestContext RequestContext { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            string password = Helpers.GlobalHelper.RequestEncodedParam("password");
            string username = Helpers.GlobalHelper.RequestEncodedParam("username");


            var login = new LibLogic.Login(username, password);

            try
            {
                login.Execute();
            }
            catch (LibLogic.Exceptions.InvalidDataException ex)
            {
                context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return;
            }

            Helpers.SessionVariables.Instance.LoggedIn = login.LoggedIn;
            Helpers.SessionVariables.Instance.Username = username;
            Helpers.SessionVariables.Instance.UserId = login.UserId;
            Helpers.SessionVariables.Instance.IsAdmin = login.IsAdmin;

            if (Helpers.SessionVariables.Instance.LoggedIn)
            {
                // if payments have expired or were never setup prompt the user
                // to setup payments
                var paymets = new LibLogic.Payments.Payment(Helpers.SessionVariables.Instance.UserId);
                if (paymets.IsExpired())
                {
                    context.Response.StatusCode = 250;
                }
                else
                {
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                }
            }
            else
            {
                context.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
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