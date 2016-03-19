using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace VpnSite.Controllers
{
    public class ApiV2Controller : Controller
    {
    
        Helpers.ISessionVariables sessionVars;

        public ApiV2Controller()
        {
            this.sessionVars = new Helpers.SessionVariables();
        }

        public ApiV2Controller(Helpers.ISessionVariables sessionVars)
        {
            this.sessionVars = sessionVars;
        }

        public ActionResult Index()
        {
            return View();
        }

        private string[] ParseAuthHeader(string authHeader)
        {
            // Check this is a Basic Auth header
            if (authHeader == null || authHeader.Length == 0 || !authHeader.StartsWith("Basic"))
                return null;

            // Pull out the Credentials with are seperated by ':' and Base64 encoded
            string base64Credentials = authHeader.Substring(6);
            string[] credentials = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Credentials)).Split(new char[] { ':' });

            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[0]))
                return null;

            // Okay this is the credentials
            return credentials;
        }

        private bool IsAuthenticateUserWithToken(HttpContextBase context, out int UserId)
        {

            UserId = -1;
            if (!context.Request.Headers.AllKeys.Contains("VpnAuthToken", StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }
            if (!context.Request.Headers.AllKeys.Contains("VpnUserId", StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }
                
            string token = context.Request.Headers["VpnAuthToken"];
            int uid = -1;
            int.TryParse(context.Request.Headers["VpnUserId"], out uid);
            var api = new LibLogic.Accounts.UserApiTokens();
            var data = api.Retrieve(uid);
           
            if (data.Token1 != token)
            {
                LibLogic.Helpers.Logging.Log("data.Token1 != token", false);
                return false;
            }

            if (data.Token1ExpireTime <= DateTime.UtcNow)
            {
                LibLogic.Helpers.Logging.Log("data.Token1ExpireTime <= DateTime.UtcNow", false);
                return false;
            }

            UserId = uid;
            return true;
        }

        [HttpPost]
        public ContentResult Auth()
        {

            // LibLogic.DTO.ApiAuthResponse results;

            try
            {

                if (!HttpContext.Request.Headers.AllKeys.Contains("Authorization", StringComparer.OrdinalIgnoreCase))
                {
                    HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    return Content("Authorization not sent");
                }

                string authHeader = HttpContext.Request.Headers["Authorization"]; 
                var creds = ParseAuthHeader(authHeader);


                var login = new LibLogic.Login(creds[0], creds[1]);

              
                try
                {
                    login.Execute();
                }
                catch (LibLogic.Exceptions.InvalidDataException ex)
                {
                    HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    LibLogic.Helpers.Logging.Log(ex);
                    return Content("InternalServerError");

                }


                if (!login.LoggedIn)
                {
                    HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    return Content("Unauthorized");
                }
                sessionVars.LoggedIn = login.LoggedIn;
                sessionVars.IsAdmin = login.IsAdmin;
                sessionVars.UserId = login.UserId;
                sessionVars.Username = login.Username;

         

                var toks = new LibLogic.Accounts.UserApiTokens();
                var tokData = toks.Retrieve(login.UserId);

                var results = new LibLogic.DTO.ApiAuthResponse()
                {
                    Token1 = tokData.Token1,
                    Token2 = tokData.Token2,
                    Token1ExpireUtc = tokData.Token1ExpireTime,
                    Token2ExpireUtc = tokData.Token2ExpireTime,
                    UserId = sessionVars.UserId
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(results);

                HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;   
                return Content(json);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                LibLogic.Helpers.Logging.Log(ex);
                return Content("InternalServerError");
            }
        }

        [HttpPost]
        public ContentResult Servers()
        {
            try
            {
                int userid = -1;
                if (!IsAuthenticateUserWithToken(HttpContext, out userid))
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    LibLogic.Helpers.Logging.Log("IsAuthenticateUserWithToken is false", false);
                    return Content("Unauthorized");
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                LibLogic.Helpers.Logging.Log(ex);
                return Content("InternalServerError " + ex.Message + ex.StackTrace);
            }

            try
            {

                var details = new LibLogic.Accounts.ServerDetails();
                             
                string data = Newtonsoft.Json.JsonConvert.SerializeObject(details.Info);
                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Content(data);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                LibLogic.Helpers.Logging.Log(ex);
                return Content("InternalServerError");
            }
        }


        [HttpPost]
        public ContentResult ChangeServer()
        {
            throw new NotImplementedException();
        }


        [HttpPost]
        public ContentResult DownloadOpenVpnCert()
        {
            int userid = -1;
            try
            {

                if (!IsAuthenticateUserWithToken(HttpContext, out userid))
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    LibLogic.Helpers.Logging.Log("IsAuthenticateUserWithToken is false", false);
                    return Content("Unauthorized");
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                LibLogic.Helpers.Logging.Log(ex);
                return Content("InternalServerError " + ex.Message + ex.StackTrace);
            }

            try
            {
                string data = WriteZippedCertsToString(userid);
                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Content(data);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                LibLogic.Helpers.Logging.Log(ex);
                return Content("InternalServerError");
            }
        }


        private string WriteZippedCertsToString(int userid)
        {
            var dl = new LibLogic.OpenVpn.CertsOpenVpnDownload();
            var fileBytes = dl.UploadToClient(userid);

            return Convert.ToBase64String(fileBytes);

        }
    }
}
