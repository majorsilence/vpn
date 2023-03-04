﻿using System;
using System.Net;
using System.Threading.Tasks;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Majorsilence.Vpn.Site.Controllers;

public class SignupController : Controller
{
    private readonly IEmail email;
    private ILogger _logger;

    public SignupController(IEmail email, ILogger logger)
    {
        this.email = email;
        _logger = logger;
    }

    public ActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task CreateUser(string email, string emailconfirm, string password, string passwordconfirm,
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
            await account.ExecuteAsync();

            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        }
        catch (InvalidDataException ide)
        {
            _logger.LogError(ide, "CreateUser InvalidDataException");
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateUser");
            HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}