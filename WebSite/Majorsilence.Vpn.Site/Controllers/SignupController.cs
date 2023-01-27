using System;
using System.Net;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class SignupController : Controller
{
    private readonly IEmail email;

    public SignupController(IEmail email)
    {
        this.email = email;
    }

    public ActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public void CreateUser(string email, string emailconfirm, string password, string passwordconfirm,
        string firstname, string lastname, string betakey)
    {
        try
        {
            var account = new CreateAccount(
                new CreateAccountInfo
                {
                    Email = email,
                    EmailConfirm = emailconfirm,
                    Firstname = firstname,
                    Lastname = lastname,
                    Password = password,
                    PasswordConfirm = passwordconfirm,
                    BetaKey = betakey
                },
                this.email
            );
            account.Execute();

            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        }
        catch (InvalidDataException ide)
        {
            Logging.Log(ide);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}