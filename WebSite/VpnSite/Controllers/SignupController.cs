using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VpnSite.Controllers
{
    public class SignupController : Controller
    {
        public ActionResult Index()
        {
            return View ();
        }

        [HttpPost]
        public void CreateUser()
        {
   
            try
            {
                string password = Helpers.GlobalHelper.RequestEncodedParam("password");
                string confirmPassword = Helpers.GlobalHelper.RequestEncodedParam("passwordconfirm");
                string email = Helpers.GlobalHelper.RequestEncodedParam("email");
                string confirmEmail = Helpers.GlobalHelper.RequestEncodedParam("emailconfirm");
                string firstname = Helpers.GlobalHelper.RequestEncodedParam("firstname");
                string lastname = Helpers.GlobalHelper.RequestEncodedParam("lastname");
                string betakey = Helpers.GlobalHelper.RequestEncodedParam("betakey");

                var emailFake = new LibLogic.Email.LiveEmail();

                var account = new LibLogic.Accounts.CreateAccount(
                    new LibLogic.Accounts.CreateAccountInfo()
                {
                    Email = email,
                    EmailConfirm = confirmEmail,
                    Firstname = firstname,
                    Lastname = lastname,
                    Password = password,
                    PasswordConfirm = confirmPassword,
                    BetaKey = betakey
                }, 
                    emailFake
                );
                account.Execute();

                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            }
            catch (LibLogic.Exceptions.InvalidDataException ide)
            {
                LibLogic.Helpers.Logging.Log(ide);
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                this.HttpContext.Response.Write(ide.Message);
            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }
                
        }

    }
}
