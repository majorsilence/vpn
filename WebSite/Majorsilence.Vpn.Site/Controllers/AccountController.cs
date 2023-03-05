using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Logic.OpenVpn;
using Majorsilence.Vpn.Logic.Payments;
using Majorsilence.Vpn.Logic.Ppp;
using Majorsilence.Vpn.Logic.Ssh;
using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NuGet.Common;

namespace Majorsilence.Vpn.Site.Controllers;

public class AccountController : Controller
{
    private readonly IEmail email;
    private readonly IStringLocalizer<AccountController> localizer;
    private readonly ISessionVariables sessionInstance;
    private readonly ILogger<AccountController> _logger;
    private readonly ActionLog _actionLog;
    private readonly DatabaseSettings _dbSettings;

    public AccountController(IEmail email, ISessionVariables sessionInstance,
        IStringLocalizer<AccountController> localizer,
        ILogger<AccountController> logger,
        ActionLog actionLog,
        DatabaseSettings dbSettings)
    {
        this.email = email;
        this.sessionInstance = sessionInstance;
        this.localizer = localizer;
        _logger = logger;
        _actionLog = actionLog;
        _dbSettings = dbSettings;
    }

    public ActionResult Index()
    {
        var acct = new Account(sessionInstance.UserId, _logger, _dbSettings)
        {
            SessionVariables = sessionInstance
        };
        ViewData["IsLoggedIn"] = sessionInstance.LoggedIn.ToString().ToLower();
        return View(acct);
    }

    public ActionResult Settings()
    {
        ViewData["IsAdmin"] = sessionInstance.IsAdmin;
        return View(new CustomViewLayout(sessionInstance));
    }

    [HttpPost]
    public async Task CancelSubscription()
    {
        if (sessionInstance.LoggedIn == false) return;

        HttpContext.Response.ContentType = "text/html";
        try
        {
            var pay = new StripePayment(sessionInstance.UserId, email, _dbSettings);
            await pay.CancelSubscription();
            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;

             _actionLog.Log("Subscription Cancelled", sessionInstance.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CancelSubscription");
            HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }


    [HttpPost]
    public void Charge(string stripeToken, string discount)
    {
        if (sessionInstance.LoggedIn == false) return;

        // Test credit card number: 4242 4242 4242 4242

        var message = "";
        try
        {
            var pay = new StripePayment(sessionInstance.UserId,
                email, _dbSettings);
            pay.MakePayment(stripeToken, discount);

            _actionLog.Log("Payment made", sessionInstance.UserId);

            // fixme: do not use Task.Run
            Task.Run(() => SetDefaultVpnServer());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Charge error");
            message = "fail";
        }

        HttpContext.Response.Redirect("/charge?status=" + message, false);
    }


    private void SetDefaultVpnServer()
    {
        _actionLog.Log("Attempt to set default vpn server after payment made",
            sessionInstance.UserId);
        try
        {
            var details = new ServerDetails(_dbSettings);

            using (var sshNewServer =
                   new LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
            using (var sshRevokeServer =
                   new LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
            using (var sftp = new LiveSftp(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
            {
                var cert = new CertsOpenVpnGenerateCommand(sessionInstance.UserId,
                    details.Info.First().VpnServerId, sshNewServer, sshRevokeServer, sftp, _dbSettings,
                    _actionLog);
                cert.Execute();
            }

            using (var sshNewServer =
                   new LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
            using (var sshRevokeServer =
                   new LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
            {
                var pptp = new ManagePPTP(sessionInstance.UserId,
                    details.Info.First().VpnServerId, sshNewServer, sshRevokeServer,
                    _dbSettings);
                pptp.AddUser();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            _actionLog.Log("Failed to set default vpn server after payment made",
                sessionInstance.UserId);
        }
    }


    [HttpPost]
    public void UpdateProfile(string email, string firstname, string lastname)
    {
        HttpContext.Response.ContentType = "text/html";
        if (sessionInstance.LoggedIn == false)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        var update = new UserInfo(sessionInstance.UserId, _logger, _dbSettings);
        try
        {
            update.UpdateProfile(email, firstname, lastname);

            _actionLog.Log(string.Format(
                    "Profile Update - Email -> {0} - First Name -> {1} - Last Name -> {2}",
                    email, firstname, lastname),
                sessionInstance.UserId);

            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        }
        catch (InvalidDataException ide)
        {
            _logger.LogError(ide, "UpdateProfile");
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        catch (EmailAddressAlreadyUsedException eaaue)
        {
            _logger.LogError(eaaue, "UpdateProfile already used email address");
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }

    [HttpPost]
    public void UpdatePassword(string oldpassword, string newpassword, string confirmnewpassword)
    {
        HttpContext.Response.ContentType = "text/html";
        if (sessionInstance.LoggedIn == false)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        var update = new UserInfo(sessionInstance.UserId, _logger, _dbSettings);
        try
        {
            update.UpdatePassword(oldpassword, newpassword, confirmnewpassword);
            _actionLog.Log("Password Changed",
                sessionInstance.UserId);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        }
        catch (InvalidDataException ide)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}