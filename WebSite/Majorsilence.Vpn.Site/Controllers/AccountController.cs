using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Logic.OpenVpn;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.Extensions.Localization;

namespace Majorsilence.Vpn.Site.Controllers;

public class AccountController : Controller
{
    private readonly IEmail email;
    private readonly ISessionVariables sessionInstance;
    private readonly IStringLocalizer<AccountController> localizer;

    public AccountController(IEmail email, ISessionVariables sessionInstance,
        IStringLocalizer<AccountController> localizer)
    {
        this.email = email;
        this.sessionInstance = sessionInstance;
        this.localizer = localizer;
    }

    public ActionResult Index()
    {
        var acct = new Models.Account(sessionInstance.UserId)
        {
            SessionVariables = sessionInstance
        };
        ViewData["IsLoggedIn"] = sessionInstance.LoggedIn.ToString().ToLower();
        return View(acct);
    }

    public ActionResult Settings()
    {
        ViewData["IsAdmin"] = sessionInstance.IsAdmin;
        return View(new Models.CustomViewLayout(sessionInstance));
    }

    [HttpPost]
    public void CancelSubscription()
    {
        if (sessionInstance.LoggedIn == false) return;

        HttpContext.Response.ContentType = "text/html";
        try
        {
            var pay = new Logic.Payments.StripePayment(sessionInstance.UserId, email);
            pay.CancelSubscription();
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;

            Logic.ActionLog.Log_BackgroundThread("Subscription Cancelled", sessionInstance.UserId);
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
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
            var pay = new Logic.Payments.StripePayment(sessionInstance.UserId,
                email);
            pay.MakePayment(stripeToken, discount);

            Logic.ActionLog.Log_BackgroundThread("Payment made", sessionInstance.UserId);


            Task.Run(() => SetDefaultVpnServer());
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            message = "fail";
        }

        HttpContext.Response.Redirect("/charge?status=" + message, false);
    }


    private void SetDefaultVpnServer()
    {
        Logic.ActionLog.Log_BackgroundThread("Attempt to set default vpn server after payment made",
            sessionInstance.UserId);
        try
        {
            var details = new Logic.Accounts.ServerDetails();

            using (var sshNewServer =
                   new Logic.Ssh.LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
            using (var sshRevokeServer =
                   new Logic.Ssh.LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
            using (var sftp = new Logic.Ssh.LiveSftp(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
            {
                var cert = new CertsOpenVpnGenerateCommand(sessionInstance.UserId,
                    details.Info.First().VpnServerId, sshNewServer, sshRevokeServer, sftp);
                cert.Execute();
            }

            using (var sshNewServer =
                   new Logic.Ssh.LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
            using (var sshRevokeServer =
                   new Logic.Ssh.LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
            {
                var pptp = new Logic.Ppp.ManagePPTP(sessionInstance.UserId,
                    details.Info.First().VpnServerId, sshNewServer, sshRevokeServer);
                pptp.AddUser();
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            Logic.ActionLog.Log_BackgroundThread("Failed to set default vpn server after payment made",
                sessionInstance.UserId);
        }
    }


    [HttpPost]
    public void UpdateProfile(string email, string firstname, string lastname)
    {
        HttpContext.Response.ContentType = "text/html";
        if (sessionInstance.LoggedIn == false)
        {
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
            return;
        }

        var update = new Logic.Accounts.UserInfo(sessionInstance.UserId);
        try
        {
            update.UpdateProfile(email, firstname, lastname);

            Logic.ActionLog.Log_BackgroundThread(string.Format(
                    "Profile Update - Email -> {0} - First Name -> {1} - Last Name -> {2}",
                    email, firstname, lastname),
                sessionInstance.UserId);

            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
        }
        catch (Logic.Exceptions.InvalidDataException ide)
        {
            Logging.Log(ide);
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
        }
        catch (Logic.Exceptions.EmailAddressAlreadyUsedException eaaue)
        {
            Logging.Log(eaaue);
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
        }
    }

    [HttpPost]
    public void UpdatePassword(string oldpassword, string newpassword, string confirmnewpassword)
    {
        HttpContext.Response.ContentType = "text/html";
        if (sessionInstance.LoggedIn == false)
        {
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
            return;
        }

        var update = new Logic.Accounts.UserInfo(sessionInstance.UserId);
        try
        {
            update.UpdatePassword(oldpassword, newpassword, confirmnewpassword);
            Logic.ActionLog.Log_BackgroundThread("Password Changed",
                sessionInstance.UserId);
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
        }
        catch (Logic.Exceptions.InvalidDataException ide)
        {
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
        }
    }
}