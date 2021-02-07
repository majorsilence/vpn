using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LibLogic.Helpers;
using LibLogic.OpenVpn;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            var acct = new Models.Account();
            return View(acct);
        }

        public ActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        public void CancelSubscription()
        {

            if (Helpers.SessionVariables.Instance.LoggedIn == false)
            {
                return;
            }

            this.HttpContext.Response.ContentType = "text/html";
            try
            {
               
                var pay = new LibLogic.Payments.StripePayment(Helpers.SessionVariables.Instance.UserId, new LibLogic.Email.LiveEmail());
                pay.CancelSubscription();
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                LibLogic.ActionLog.Log_BackgroundThread("Subscription Cancelled", Helpers.SessionVariables.Instance.UserId);
            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }


           


        }


        [HttpPost]
        public void Charge(string stripeToken, string discount)
        {

            if (Helpers.SessionVariables.Instance.LoggedIn == false)
            {
                return;
            }

            // Test credit card number: 4242 4242 4242 4242

            string message = "";
            try
            {


                var pay = new LibLogic.Payments.StripePayment(Helpers.SessionVariables.Instance.UserId, 
                              new LibLogic.Email.LiveEmail());
                pay.MakePayment(stripeToken, discount);

                LibLogic.ActionLog.Log_BackgroundThread("Payment made", Helpers.SessionVariables.Instance.UserId);
              

                Task.Run(() => SetDefaultVpnServer());

            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                message = "fail";
            }

            this.HttpContext.Response.Redirect("/charge?status=" + message, false);

        }


        private void SetDefaultVpnServer()
        {
            LibLogic.ActionLog.Log_BackgroundThread("Attempt to set default vpn server after payment made", 
                Helpers.SessionVariables.Instance.UserId);
            try
            {
                var details = new LibLogic.Accounts.ServerDetails();

                using (var sshNewServer = new LibLogic.Ssh.LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
                using (var sshRevokeServer = new LibLogic.Ssh.LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
                using (var sftp = new LibLogic.Ssh.LiveSftp(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
                {
                    var cert = new CertsOpenVpnGenerateCommand(Helpers.SessionVariables.Instance.UserId,
                                   details.Info.First().VpnServerId, sshNewServer, sshRevokeServer, sftp);
                    cert.Execute();
                    
                }

                using (var sshNewServer = new LibLogic.Ssh.LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
                using (var sshRevokeServer = new LibLogic.Ssh.LiveSsh(SiteInfo.SshPort, SiteInfo.VpnSshUser, SiteInfo.VpnSshPassword))
                {
                    var pptp = new LibLogic.Ppp.ManagePPTP(Helpers.SessionVariables.Instance.UserId,
                                   details.Info.First().VpnServerId, sshNewServer, sshRevokeServer);
                    pptp.AddUser();
                }
            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                LibLogic.ActionLog.Log_BackgroundThread("Failed to set default vpn server after payment made", Helpers.SessionVariables.Instance.UserId);
            }
        }


        [HttpPost]
        public void UpdateProfile(string email, string firstname, string lastname)
        {
            this.HttpContext.Response.ContentType = "text/html";
            if (Helpers.SessionVariables.Instance.LoggedIn == false)
            {
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                return;
            }
                
            var update = new LibLogic.Accounts.UserInfo(Helpers.SessionVariables.Instance.UserId);
            try
            {
                update.UpdateProfile(email, firstname, lastname);

                LibLogic.ActionLog.Log_BackgroundThread(string.Format("Profile Update - Email -> {0} - First Name -> {1} - Last Name -> {2}", 
                    email, firstname, lastname), 
                    Helpers.SessionVariables.Instance.UserId);

                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            }
            catch (LibLogic.Exceptions.InvalidDataException ide)
            {
                LibLogic.Helpers.Logging.Log(ide);
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }
            catch (LibLogic.Exceptions.EmailAddressAlreadyUsedException eaaue)
            {
                LibLogic.Helpers.Logging.Log(eaaue);
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

        }

        [HttpPost]
        public void UpdatePassword()
        {
            this.HttpContext.Response.ContentType = "text/html";
            if (Helpers.SessionVariables.Instance.LoggedIn == false)
            {
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                return;
            }

            string oldpassword = Helpers.GlobalHelper.RequestEncodedParam("oldpassword");
            string newpassword = Helpers.GlobalHelper.RequestEncodedParam("newpassword");
            string confirmnewpassword = Helpers.GlobalHelper.RequestEncodedParam("confirmnewpassword");

            var update = new LibLogic.Accounts.UserInfo(Helpers.SessionVariables.Instance.UserId);
            try
            {
                update.UpdatePassword(oldpassword, newpassword, confirmnewpassword);
                LibLogic.ActionLog.Log_BackgroundThread("Password Changed", 
                    Helpers.SessionVariables.Instance.UserId);
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            }
            catch (LibLogic.Exceptions.InvalidDataException ide)
            {
                this.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }


        }


    }
}
