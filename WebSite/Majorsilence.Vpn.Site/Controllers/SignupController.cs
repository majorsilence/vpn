using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Majorsilence.Vpn.Logic.Email;
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
            var account = new Logic.Accounts.CreateAccount(
                new Logic.Accounts.CreateAccountInfo()
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

            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
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