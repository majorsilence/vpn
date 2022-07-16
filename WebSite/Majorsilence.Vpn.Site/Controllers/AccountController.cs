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

namespace Majorsilence.Vpn.Site.Controllers
{
    public class AccountController : Controller
    {
        readonly IEmail email;
        readonly ISessionVariables sessionInstance;
        private readonly IStringLocalizer<AccountController> localizer;
        public AccountController(IEmail email, ISessionVariables sessionInstance, IStringLocalizer<AccountController> localizer)
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

            if (sessionInstance.LoggedIn == false)
            {
                return;
            }

            this.HttpContext.Response.ContentType = "text/html";
            try
            {

                var pay = new Majorsilence.Vpn.Logic.Payments.StripePayment(sessionInstance.UserId, email);
                pay.CancelSubscription();
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                Majorsilence.Vpn.Logic.ActionLog.Log_BackgroundThread("Subscription Cancelled", sessionInstance.UserId);
            }
            catch (Exception ex)
            {
                Majorsilence.Vpn.Logic.Helpers.Logging.Log(ex);
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }

        }


        [HttpPost]
        public void Charge(string stripeToken, string discount)
        {

            if (sessionInstance.LoggedIn == false)
            {
                return;
            }

            // Test credit card number: 4242 4242 4242 4242

            string message = "";
            try
            {


                var pay = new Majorsilence.Vpn.Logic.Payments.StripePayment(sessionInstance.UserId,
                              email);
                pay.MakePayment(stripeToken, discount);

                Majorsilence.Vpn.Logic.ActionLog.Log_BackgroundThread("Payment made", sessionInstance.UserId);


                Task.Run(() => SetDefaultVpnServer());

            }
            catch (Exception ex)
            {
                Majorsilence.Vpn.Logic.Helpers.Logging.Log(ex);
                message = "fail";
            }

            this.HttpContext.Response.Redirect("/charge?status=" + message, false);

        }


        private void SetDefaultVpnServer()
        {
            Majorsilence.Vpn.Logic.ActionLog.Log_BackgroundThread("Attempt to set default vpn server after payment made",
                sessionInstance.UserId);
            try
            {
                var details = new Majorsilence.Vpn.Logic.Accounts.ServerDetails();

                using (var sshNewServer = new Majorsilence.Vpn.Logic.Ssh.LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
                using (var sshRevokeServer = new Majorsilence.Vpn.Logic.Ssh.LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
                using (var sftp = new Majorsilence.Vpn.Logic.Ssh.LiveSftp(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
                {
                    var cert = new CertsOpenVpnGenerateCommand(sessionInstance.UserId,
                                   details.Info.First().VpnServerId, sshNewServer, sshRevokeServer, sftp);
                    cert.Execute();

                }

                using (var sshNewServer = new Majorsilence.Vpn.Logic.Ssh.LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
                using (var sshRevokeServer = new Majorsilence.Vpn.Logic.Ssh.LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
                {
                    var pptp = new Majorsilence.Vpn.Logic.Ppp.ManagePPTP(sessionInstance.UserId,
                                   details.Info.First().VpnServerId, sshNewServer, sshRevokeServer);
                    pptp.AddUser();
                }
            }
            catch (Exception ex)
            {
                Majorsilence.Vpn.Logic.Helpers.Logging.Log(ex);
                Majorsilence.Vpn.Logic.ActionLog.Log_BackgroundThread("Failed to set default vpn server after payment made", sessionInstance.UserId);
            }
        }


        [HttpPost]
        public void UpdateProfile(string email, string firstname, string lastname)
        {
            this.HttpContext.Response.ContentType = "text/html";
            if (sessionInstance.LoggedIn == false)
            {
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                return;
            }

            var update = new Majorsilence.Vpn.Logic.Accounts.UserInfo(sessionInstance.UserId);
            try
            {
                update.UpdateProfile(email, firstname, lastname);

                Majorsilence.Vpn.Logic.ActionLog.Log_BackgroundThread(string.Format("Profile Update - Email -> {0} - First Name -> {1} - Last Name -> {2}",
                    email, firstname, lastname),
                    sessionInstance.UserId);

                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            }
            catch (Majorsilence.Vpn.Logic.Exceptions.InvalidDataException ide)
            {
                Majorsilence.Vpn.Logic.Helpers.Logging.Log(ide);
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }
            catch (Majorsilence.Vpn.Logic.Exceptions.EmailAddressAlreadyUsedException eaaue)
            {
                Majorsilence.Vpn.Logic.Helpers.Logging.Log(eaaue);
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

        }

        [HttpPost]
        public void UpdatePassword(string oldpassword, string newpassword, string confirmnewpassword)
        {
            this.HttpContext.Response.ContentType = "text/html";
            if (sessionInstance.LoggedIn == false)
            {
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                return;
            }

            var update = new Majorsilence.Vpn.Logic.Accounts.UserInfo(sessionInstance.UserId);
            try
            {
                update.UpdatePassword(oldpassword, newpassword, confirmnewpassword);
                Majorsilence.Vpn.Logic.ActionLog.Log_BackgroundThread("Password Changed",
                    sessionInstance.UserId);
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            }
            catch (Majorsilence.Vpn.Logic.Exceptions.InvalidDataException ide)
            {
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }


        }


    }
}
