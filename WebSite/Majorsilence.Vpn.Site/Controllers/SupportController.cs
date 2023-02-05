using System;
using System.Net;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Majorsilence.Vpn.Site.Controllers;

public class SupportController : Controller
{
    private readonly IEmail email;
    private readonly ISessionVariables sessionInstance;
    private readonly ILogger<SupportController> _logger;

    public SupportController(IEmail email, ISessionVariables sessionInstance,
        ILogger<SupportController> logger)
    {
        this.email = email;
        this.sessionInstance = sessionInstance;
        _logger = logger;
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
            email.SendMail_BackgroundThread(supportrequest, subject, "peter@majorsilence.com", false);

            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            HttpContext.Response.Redirect("/support/thankyou", false);
        }
        catch (InvalidDataException ide)
        {
            _logger.LogError(ide, "InvalidDataException");
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}