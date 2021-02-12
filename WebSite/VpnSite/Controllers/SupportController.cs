using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LibLogic.Email;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class SupportController : Controller
    {
        readonly IEmail email;
        readonly ISessionVariables sessionInstance;
        public SupportController(IEmail email, ISessionVariables sessionInstance)
        {
            this.email = email;
            this.sessionInstance = sessionInstance;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ThankYou()
        {
            return View();
        }

        [HttpPost]
        public void Submit(string subject, string supportrequest)
        {

            if (sessionInstance.LoggedIn == false)
            {
                return;
            }


            try
            {
                supportrequest = "User Id: " + sessionInstance.UserId + System.Environment.NewLine +
                "Email: " + sessionInstance.Username + System.Environment.NewLine + System.Environment.NewLine +
                supportrequest;

                // TODO: inject the support address from appsettings.json
                email.SendMail_BackgroundThread(supportrequest, subject, "peter@majorsilence.com", false, null, LibLogic.Email.EmailTemplates.None);

                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                this.HttpContext.Response.Redirect("/support/thankyou", false);
            }
            catch (LibLogic.Exceptions.InvalidDataException ide)
            {
                LibLogic.Helpers.Logging.Log(ide);
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }


        }

    }
}
