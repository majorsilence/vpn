using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class GenericController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public FileResult DownloadOpenVpnCert()
        {
            if (Helpers.SessionVariables.Instance.LoggedIn == false)
            {
                return null;
            }

            var dl = new LibLogic.OpenVpn.CertsOpenVpnDownload();
            var fileBytes = dl.UploadToClient(Helpers.SessionVariables.Instance.UserId);

            return File(fileBytes, "application/zip", "Certs.zip");
        }

        [HttpPost]
        public JsonResult SaveUserVpnServer(int vpnId)
        {

            if (Helpers.SessionVariables.Instance.LoggedIn == false)
            {
                return null;
            }


            try
            {
                VpnServer(vpnId);
            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);

                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return null;
            }


            try
            {
                PptpServer(vpnId);
            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);


                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;

                using (var ssh = new LibLogic.Ssh.LiveSsh(LibLogic.Helpers.SiteInfo.SshPort,
                                     LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
                {
                    var revokeOVPN = new LibLogic.OpenVpn.CertsOpenVpnRevokeCommand(Helpers.SessionVariables.Instance.UserId, ssh);
                    revokeOVPN.Execute();
                }

                return null;
            }

            var model = new Models.Setup();

            this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;


            return Json(model);

        }

        private void VpnServer(int vpnServerId)
        {

            using (var sshNewServer = new LibLogic.Ssh.LiveSsh(LibLogic.Helpers.SiteInfo.SshPort,
                                          LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
            using (var sshRevokeServer = new LibLogic.Ssh.LiveSsh(LibLogic.Helpers.SiteInfo.SshPort,
                                             LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
            using (var sftp = new LibLogic.Ssh.LiveSftp(LibLogic.Helpers.SiteInfo.SshPort,
                                  LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
            {
                var cert = new LibLogic.OpenVpn.CertsOpenVpnGenerateCommand(Helpers.SessionVariables.Instance.UserId,
                               vpnServerId, sshNewServer, sshRevokeServer, sftp);
                cert.Execute();
            }

        }

        private void PptpServer(int vpnServerId)
        {
            using (var sshNewServer = new LibLogic.Ssh.LiveSsh(LibLogic.Helpers.SiteInfo.SshPort,
                                          LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
            using (var sshRevokeServer = new LibLogic.Ssh.LiveSsh(LibLogic.Helpers.SiteInfo.SshPort,
                                             LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
            {
                var pptp = new LibLogic.Ppp.ManagePPTP(Helpers.SessionVariables.Instance.UserId, vpnServerId,
                               sshNewServer, sshRevokeServer);
                pptp.AddUser();
            }
        }

        private void IpsecServer(int vpnServerId)
        {
            using (var sshNewServer = new LibLogic.Ssh.LiveSsh(LibLogic.Helpers.SiteInfo.SshPort,
                                          LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
            using (var sshRevokeServer = new LibLogic.Ssh.LiveSsh(LibLogic.Helpers.SiteInfo.SshPort,
                                             LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
            {
                var ipsec = new LibLogic.Ppp.IpSec(Helpers.SessionVariables.Instance.UserId, vpnServerId, sshNewServer,
                                sshRevokeServer);
                ipsec.AddUser();
            }
        }

        [HttpPost]
        public void LoginValidation(string username, string password)
        {
            var login = new LibLogic.Login(username, password);

            try
            {
                login.Execute();
            }
            catch (LibLogic.Exceptions.InvalidDataException)
            {
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return;
            }

            Helpers.SessionVariables.Instance.LoggedIn = login.LoggedIn;
            Helpers.SessionVariables.Instance.Username = username;
            Helpers.SessionVariables.Instance.UserId = login.UserId;
            Helpers.SessionVariables.Instance.IsAdmin = login.IsAdmin;

            if (Helpers.SessionVariables.Instance.LoggedIn)
            {
                // if payments have expired or were never setup prompt the user
                // to setup payments
                var paymets = new LibLogic.Payments.Payment(Helpers.SessionVariables.Instance.UserId);
                if (paymets.IsExpired())
                {
                    this.HttpContext.Response.StatusCode = 250;
                }
                else
                {
                    this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                }
            }
            else
            {
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
            }

        }

        [HttpPost]
        public void ResetPasswordSend(string username)
        {

            try
            {
                var email = new LibLogic.Email.LiveEmail();
                var resetPSW = new LibLogic.Accounts.ResetPassword(email);
                resetPSW.sendPasswordLink(username);
            }
            catch (LibLogic.Exceptions.InvalidDataException ide)
            {
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
            }
            catch (Exception ex)
            {
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }


        }

        [HttpPost]
        public void ResetCodeValidation(string code, string cnewpsw, string newpsw)
        {

            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(cnewpsw) && !string.IsNullOrEmpty(newpsw))
            {
                if (cnewpsw != newpsw)
                {
                    this.HttpContext.Response.StatusCode = 400;
                    return;
                }
                var email = new LibLogic.Email.LiveEmail();
                var resetPSW = new LibLogic.Accounts.ResetPassword(email);
                resetPSW.validateCode(code, newpsw);
                this.HttpContext.Response.StatusCode = 250;
            }
            else
            {
                this.HttpContext.Response.StatusCode = 400;
                return;
            }

        }

        public void StripeWebhook()
        {

            try
            {
                if (Helpers.SessionVariables.Instance.LoggedIn == false)
                {
                    return;
                }

                using (var stream = new StreamReader(this.HttpContext.Request.Body)) {
                    var json = stream.ReadToEnd();
                    var hook = new LibLogic.Payments.StripWebHook(json);
                    hook.Execute();

                }
                this.HttpContext.Response.StatusCode = 200;
                
            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex, true);
                this.HttpContext.Response.StatusCode = 500;
            }
        }

    }
}
