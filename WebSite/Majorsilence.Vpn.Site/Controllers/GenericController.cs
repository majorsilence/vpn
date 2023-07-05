using System;
using System.IO;
using System.Net;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Logic.OpenVpn;
using Majorsilence.Vpn.Logic.Payments;
using Majorsilence.Vpn.Logic.Ppp;
using Majorsilence.Vpn.Logic.Ssh;
using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InvalidDataException = Majorsilence.Vpn.Logic.Exceptions.InvalidDataException;

namespace Majorsilence.Vpn.Site.Controllers;

public class GenericController : Controller
{
    private readonly ActionLog _actionLog;
    private readonly DatabaseSettings _dbSettings;
    private readonly IEmail email;
    private readonly ISessionVariables sessionInstance;
    private readonly IEncryptionKeysSettings _keys;
    private readonly ILogger _logger;

    public GenericController(IEmail email, ISessionVariables sessionInstance,
        ILogger logger,
        IEncryptionKeysSettings keys,
        DatabaseSettings dbSettings,
        ActionLog actionLog)
    {
        this.email = email;
        this.sessionInstance = sessionInstance;
        _logger = logger;
        _keys = keys;
        _dbSettings = dbSettings;
        _actionLog = actionLog;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public FileResult DownloadOpenVpnCert()
    {
        if (sessionInstance.LoggedIn == false) return null;

        var dl = new CertsOpenVpnDownload(_dbSettings);
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
            _logger.LogError(ex, "SaveUserVpnServer vpnserver");

            HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return null;
        }


        try
        {
            PptpServer(vpnId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SaveUserVpnServer pptp");


            HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            using (var ssh = new LiveSsh(SiteInfo.SshPort,
                       SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
            {
                var revokeOVPN = new CertsOpenVpnRevokeCommand(sessionInstance.UserId, ssh, _dbSettings, _actionLog);
                revokeOVPN.Execute();
            }

            return null;
        }

        var model = new Setup(sessionInstance.UserId, sessionInstance.Username, _dbSettings);

        HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;


        return Json(model);
    }

    private void VpnServer(int vpnServerId)
    {
        using (var sshNewServer = new LiveSsh(SiteInfo.SshPort,
                   SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
        using (var sshRevokeServer = new LiveSsh(SiteInfo.SshPort,
                   SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
        using (var sftp = new LiveSftp(SiteInfo.SshPort,
                   SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
        {
            var cert = new CertsOpenVpnGenerateCommand(sessionInstance.UserId,
                vpnServerId, sshNewServer, sshRevokeServer, sftp, _dbSettings, _actionLog);
            cert.Execute();
        }
    }

    private void PptpServer(int vpnServerId)
    {
        using (var sshNewServer = new LiveSsh(SiteInfo.SshPort,
                   SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
        using (var sshRevokeServer = new LiveSsh(SiteInfo.SshPort,
                   SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
        {
            var pptp = new ManagePPTP(sessionInstance.UserId, vpnServerId,
                sshNewServer, sshRevokeServer, _dbSettings);
            pptp.AddUser();
        }
    }

    private void IpsecServer(int vpnServerId)
    {
        using (var sshNewServer = new LiveSsh(SiteInfo.SshPort,
                   SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
        using (var sshRevokeServer = new LiveSsh(SiteInfo.SshPort,
                   SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
        {
            var ipsec = new IpSec(sessionInstance.UserId, vpnServerId, sshNewServer,
                sshRevokeServer, _dbSettings);
            ipsec.AddUser();
        }
    }

    [HttpPost]
    public void LoginValidation(string username, string password)
    {
        var login = new Login(username, password, _logger, _dbSettings);

        try
        {
            login.Execute();
        }
        catch (InvalidDataException)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
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
            var paymets = new Payment(sessionInstance.UserId, _dbSettings);
            if (paymets.IsExpired())
                HttpContext.Response.StatusCode = 250;
            else
                HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        }
        else
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        }
    }

    [HttpPost]
    public void ResetPasswordSend(string username)
    {
        try
        {
            var resetPSW = new ResetPassword(email, _keys, _dbSettings);
            resetPSW.sendPasswordLink(username);
        }
        catch (InvalidDataException ide)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        }
        catch (Exception ex)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
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

            var resetPSW = new ResetPassword(email, _keys, _dbSettings);
            resetPSW.validateCode(code, newpsw);
            HttpContext.Response.StatusCode = 250;
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
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
                var hook = new StripWebHook(json);
                hook.Execute();
            }

            HttpContext.Response.StatusCode = 200;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StripeWebhook");
            HttpContext.Response.StatusCode = 500;
        }
    }
}