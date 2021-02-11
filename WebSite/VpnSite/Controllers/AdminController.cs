using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LibLogic.Email;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class AdminController : Controller
    {
        readonly IEmail email;
        readonly ISessionVariables sessionInstance;
        public AdminController(IEmail email, ISessionVariables sessionInstance)
        {
            this.email = email;
            this.sessionInstance = sessionInstance;
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SiteInfo(string status)
        {
            ViewData["status"] = status;
            return View();
        }

        public ActionResult Users(string status)
        {
            ViewData["status"] = status;

            if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false)
            {
                return null;
            }

            var model = new Models.Users();
            return View(model);
        }

        public ActionResult ErrorReport()
        {
            return View();
        }

        public void RemoveStripeAccount(int id, string removeaccount)
        {

            if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false)
            {
                return;
            }

            try
            {

                if (removeaccount != null && removeaccount == "yes")
                {

                    var payments = new LibLogic.Payments.StripePayment(id, email);
                    payments.CancelSubscription();
                    payments.CancelAccount();


                    Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode("User with id removed: " + id), false);
                }

            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode(ex.Message), false);
            }

        }

        [HttpPost]
        public void RemoveSubscription(int id, string removeaccount)
        {
            if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false)
            {
                return;
            }

            try
            {

                if (removeaccount != null && removeaccount == "yes")
                {
                    var payments = new LibLogic.Payments.StripePayment(id, email);
                    payments.CancelSubscription();


                    Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode("User with id removed: " + id), false);
                }

            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode(ex.Message), false);
            }
        }

        [HttpPost]
        public void RemoveUser(int id, string removeaccount)
        {

            if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false)
            {
                return;
            }

            try
            {

                if (removeaccount != null && removeaccount == "yes")
                {
                    var user = new LibLogic.Accounts.UserInfo(id);
                    user.RemoveAccount();


                    Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode("User with id removed: " + id), false);
                }

            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode(ex.Message), false);
            }

        }

        [HttpPost]
        public void ToggleAdmin(int id)
        {
            if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false)
            {
                return;
            }

            try
            {
                var modify = new LibLogic.Accounts.ModifyAccount();
                modify.ToggleIsAdmin(id);

                Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode("User with id admin mode was toggled: " + id), false);
            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode(ex.Message), false);
            }
        }


        [HttpPost]
        public void SaveSiteInfo(int siteid, string adminemail, string livesite, string sitename, string siteurl,
             int sshport, string vpnsshuser, string vpnsshpassword, string stripeapipublickey,
             string stripeapisecretkey, string currency, string stripeplanid,
             decimal monthlypaymentrate, decimal yearlypaymentrate)
        {

            if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false)
            {
                return;
            }

            try
            {
                bool islivesite = false;
                if (livesite == "on")
                {
                    // means no beta key required
                    islivesite = true;
                }

                var info = new LibPoco.SiteInfo()
                {
                    Id = siteid,
                    AdminEmail = adminemail,
                    LiveSite = islivesite,
                    SiteName = sitename,
                    SiteUrl = siteurl,
                    SshPort = sshport,
                    StripeAPISecretKey = stripeapisecretkey,
                    StripeAPIPublicKey = stripeapipublickey,
                    VpnSshPassword = vpnsshpassword,
                    VpnSshUser = vpnsshuser,
                    StripePlanId = stripeplanid,
                    Currency = currency
                };

                LibLogic.Helpers.SiteInfo.InitializeSimple(info, monthlypaymentrate, yearlypaymentrate);
                LibLogic.Helpers.SiteInfo.SaveCurrentSettingsToDb();

                Response.Redirect("/admin/siteinfo?status=ok", false);
            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                Response.Redirect("/admin/siteinfo?status=" + HttpUtility.HtmlEncode(ex.Message), false);
            }

        }
    }
}
