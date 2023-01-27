using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Majorsilence.Vpn.Site.Controllers;

public class GenericController : Controller
{
    private readonly IEmail email;
    private readonly ISessionVariables sessionInstance;

    public GenericController(IEmail email, ISessionVariables sessionInstance)
    {
        this.email = email;
        this.sessionInstance = sessionInstance;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public FileResult DownloadOpenVpnCert()
    {
        if (sessionInstance.LoggedIn == false) return null;

        var dl = new Logic.OpenVpn.CertsOpenVpnDownload();
        var fileBytes = dl.UploadToClient(sessionInstance.UserId);

        return File(fileBytes, "application/zip", "Certs.zip");
    }

    [HttpPost]
    public JsonResult SaveUserVpnServer(int vpnId)
    {
        if (sessionInstance.LoggedIn == false) return null;


        try
        {
            VpnServer(vpnId);
        }
        catch (Exception ex)
        {
            Logic.Helpers.Logging.Log(ex);

            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            return null;
        }


        try
        {
            PptpServer(vpnId);
        }
        catch (Exception ex)
        {
            Logic.Helpers.Logging.Log(ex);


            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;

            using (var ssh = new Logic.Ssh.LiveSsh(Logic.Helpers.SiteInfo.SshPort,
                       Logic.Helpers.SiteInfo.VpnSshUser, Logic.Helpers.SiteInfo.VpnSshPassword))
            {
                var revokeOVPN = new Logic.OpenVpn.CertsOpenVpnRevokeCommand(sessionInstance.UserId, ssh);
                revokeOVPN.Execute();
            }

            return null;
        }

        var model = new Models.Setup(sessionInstance.UserId, sessionInstance.Username);

        HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;


        return Json(model);
    }

    private void VpnServer(int vpnServerId)
    {
        using (var sshNewServer = new Logic.Ssh.LiveSsh(Logic.Helpers.SiteInfo.SshPort,
                   Logic.Helpers.SiteInfo.VpnSshUser, Logic.Helpers.SiteInfo.VpnSshPassword))
        using (var sshRevokeServer = new Logic.Ssh.LiveSsh(Logic.Helpers.SiteInfo.SshPort,
                   Logic.Helpers.SiteInfo.VpnSshUser, Logic.Helpers.SiteInfo.VpnSshPassword))
        using (var sftp = new Logic.Ssh.LiveSftp(Logic.Helpers.SiteInfo.SshPort,
                   Logic.Helpers.SiteInfo.VpnSshUser, Logic.Helpers.SiteInfo.VpnSshPassword))
        {
            var cert = new Logic.OpenVpn.CertsOpenVpnGenerateCommand(sessionInstance.UserId,
                vpnServerId, sshNewServer, sshRevokeServer, sftp);
            cert.Execute();
        }
    }

    private void PptpServer(int vpnServerId)
    {
        using (var sshNewServer = new Logic.Ssh.LiveSsh(Logic.Helpers.SiteInfo.SshPort,
                   Logic.Helpers.SiteInfo.VpnSshUser, Logic.Helpers.SiteInfo.VpnSshPassword))
        using (var sshRevokeServer = new Logic.Ssh.LiveSsh(Logic.Helpers.SiteInfo.SshPort,
                   Logic.Helpers.SiteInfo.VpnSshUser, Logic.Helpers.SiteInfo.VpnSshPassword))
        {
            var pptp = new Logic.Ppp.ManagePPTP(sessionInstance.UserId, vpnServerId,
                sshNewServer, sshRevokeServer);
            pptp.AddUser();
        }
    }

    private void IpsecServer(int vpnServerId)
    {
        using (var sshNewServer = new Logic.Ssh.LiveSsh(Logic.Helpers.SiteInfo.SshPort,
                   Logic.Helpers.SiteInfo.VpnSshUser, Logic.Helpers.SiteInfo.VpnSshPassword))
        using (var sshRevokeServer = new Logic.Ssh.LiveSsh(Logic.Helpers.SiteInfo.SshPort,
                   Logic.Helpers.SiteInfo.VpnSshUser, Logic.Helpers.SiteInfo.VpnSshPassword))
        {
            var ipsec = new Logic.Ppp.IpSec(sessionInstance.UserId, vpnServerId, sshNewServer,
                sshRevokeServer);
            ipsec.AddUser();
        }
    }

    [HttpPost]
    public void LoginValidation(string username, string password)
    {
        var login = new Logic.Login(username, password);

        try
        {
            login.Execute();
        }
        catch (Majorsilence.Vpn.Logic.Exceptions.InvalidDataException)
        {
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            return;
        }

        sessionInstance.LoggedIn = login.LoggedIn;
        sessionInstance.Username = username;
        sessionInstance.UserId = login.UserId;
        sessionInstance.IsAdmin = login.IsAdmin;

        if (sessionInstance.LoggedIn)
        {
            // if payments have expired or were never setup prompt the user
            // to setup payments
            var paymets = new Logic.Payments.Payment(sessionInstance.UserId);
            if (paymets.IsExpired())
                HttpContext.Response.StatusCode = 250;
            else
                HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
        }
        else
        {
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
        }
    }

    [HttpPost]
    public void ResetPasswordSend(string username)
    {
        try
        {
            var resetPSW = new Logic.Accounts.ResetPassword(email);
            resetPSW.sendPasswordLink(username);
        }
        catch (Majorsilence.Vpn.Logic.Exceptions.InvalidDataException ide)
        {
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
        }
        catch (Exception ex)
        {
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
        }
    }

    [HttpPost]
    public void ResetCodeValidation(string code, string cnewpsw, string newpsw)
    {
        if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(cnewpsw) && !string.IsNullOrEmpty(newpsw))
        {
            if (cnewpsw != newpsw)
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }

            var resetPSW = new Logic.Accounts.ResetPassword(email);
            resetPSW.validateCode(code, newpsw);
            HttpContext.Response.StatusCode = 250;
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }
    }

    public void StripeWebhook()
    {
        try
        {
            if (sessionInstance.LoggedIn == false) return;

            using (var stream = new StreamReader(HttpContext.Request.Body))
            {
                var json = stream.ReadToEnd();
                var hook = new Logic.Payments.StripWebHook(json);
                hook.Execute();
            }

            HttpContext.Response.StatusCode = 200;
        }
        catch (Exception ex)
        {
            Logic.Helpers.Logging.Log(ex, true);
            HttpContext.Response.StatusCode = 500;
        }
    }
}