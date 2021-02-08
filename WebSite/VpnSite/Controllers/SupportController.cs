using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class SupportController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ThankYou()
        {
            return View();
        }

        [HttpPost]
        public void Submit()
        {

            if (Helpers.SessionVariables.Instance.LoggedIn == false)
            {
                return;
            }


            try
            {
                string subject = Helpers.GlobalHelper.RequestEncodedParam("subject");
                string message = Helpers.GlobalHelper.RequestEncodedParam("supportrequest");

                message = "User Id: " + Helpers.SessionVariables.Instance.UserId + System.Environment.NewLine +
                "Email: " + Helpers.SessionVariables.Instance.Username + System.Environment.NewLine + System.Environment.NewLine +
                message;

                var email = new LibLogic.Email.LiveEmail();
                email.SendMail_BackgroundThread(message, subject, "peter@majorsilence.com", false, null, LibLogic.Email.EmailTemplates.None);

                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                this.HttpContext.Response.Redirect("/support/thankyou", false);
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
