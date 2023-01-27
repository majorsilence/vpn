using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class SupportController : Controller
{
    private readonly IEmail email;
    private readonly ISessionVariables sessionInstance;

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
        if (sessionInstance.LoggedIn == false) return;


        try
        {
            supportrequest = "User Id: " + sessionInstance.UserId + Environment.NewLine +
                             "Email: " + sessionInstance.Username + Environment.NewLine + Environment.NewLine +
                             supportrequest;

            // TODO: inject the support address from appsettings.json
            email.SendMail_BackgroundThread(supportrequest, subject, "peter@majorsilence.com", false, null,
                EmailTemplates.None);

            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            HttpContext.Response.Redirect("/support/thankyou", false);
        }
        catch (Logic.Exceptions.InvalidDataException ide)
        {
            Logic.Helpers.Logging.Log(ide);
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
        }
        catch (Exception ex)
        {
            Logic.Helpers.Logging.Log(ex);
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
        }
    }
}