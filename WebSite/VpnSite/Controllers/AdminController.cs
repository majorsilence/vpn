using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VpnSite.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SiteInfo()
        {
            return View();
        }

        public ActionResult Users()
        {
            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
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

        public void RemoveStripeAccount()
        {

            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return;
            }

            try
            {
                int userid;
                bool isNumeric = int.TryParse(VpnSite.Helpers.GlobalHelper.RequestParam("id").Trim().ToLower(), out userid);
                if (!isNumeric)
                {
                    Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode("invalid id"), false);
                }

                string removeaccount = Helpers.GlobalHelper.RequestEncodedParam("removeaccount");
                if (removeaccount != null && removeaccount == "yes")
                {
                   
                    var payments = new LibLogic.Payments.StripePayment(userid, new LibLogic.Email.LiveEmail());
                    payments.CancelSubscription();
                    payments.CancelAccount();


                    Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode("User with id removed: " + userid), false);
                }

            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode(ex.Message), false);
            }

        }

        [HttpPost]
        public void RemoveSubscription()
        {
            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return;
            }

            try
            {
                int userid;
                bool isNumeric = int.TryParse(VpnSite.Helpers.GlobalHelper.RequestParam("id").Trim().ToLower(), out userid);
                if (!isNumeric)
                {
                    Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode("invalid id"), false);
                }

                string removeaccount = Helpers.GlobalHelper.RequestEncodedParam("removeaccount");
                if (removeaccount != null && removeaccount == "yes")
                {
                    var payments = new LibLogic.Payments.StripePayment(userid, new LibLogic.Email.LiveEmail());
                    payments.CancelSubscription();


                    Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode("User with id removed: " + userid), false);
                }

            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode(ex.Message), false);
            }
        }

        [HttpPost]
        public void RemoveUser()
        {

            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return;
            }

            try
            {
                int userid;
                bool isNumeric = int.TryParse(VpnSite.Helpers.GlobalHelper.RequestParam("id").Trim().ToLower(), out userid);
                if (!isNumeric)
                {
                    Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode("invalid id"), false);
                }
 
                string removeaccount = Helpers.GlobalHelper.RequestEncodedParam("removeaccount");
                if (removeaccount != null && removeaccount == "yes")
                {
                    var user = new LibLogic.Accounts.UserInfo(userid);
                    user.RemoveAccount();


                    Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode("User with id removed: " + userid), false);
                }

            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode(ex.Message), false);
            }

        }

        [HttpPost]
        public void ToggleAdmin()
        {
            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return;
            }

            try
            {
                int userid;
                bool isNumeric = int.TryParse(VpnSite.Helpers.GlobalHelper.RequestParam("id").Trim().ToLower(), out userid);
                if (!isNumeric)
                {
                    Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode("invalid id"), false);
                }

                var modify = new LibLogic.Accounts.ModifyAccount();
                modify.ToggleIsAdmin(userid);
       
                Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode("User with id admin mode was toggled: " + userid), false);
            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode(ex.Message), false);
            }
        }


        [HttpPost]
        public void SaveSiteInfo()
        {

            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return;
            }

            try
            {
                string adminemail = Helpers.GlobalHelper.RequestEncodedParam("adminemail");
                bool islivesite = false;
                string livesite = Helpers.GlobalHelper.RequestEncodedParam("livesite");
                if (livesite == "on")
                {
                    // means no beta key required
                    islivesite = true;
                }

                int id = int.Parse(Helpers.GlobalHelper.RequestParam("siteid"));
                string sitename = Helpers.GlobalHelper.RequestEncodedParam("sitename");
                string siteurl = Helpers.GlobalHelper.RequestEncodedParam("siteurl");
                int sshport = int.Parse(Helpers.GlobalHelper.RequestEncodedParam("sshport"));
                string stripeapisecretkey = Helpers.GlobalHelper.RequestEncodedParam("stripeapisecretkey");
                string stripeapipublickey = Helpers.GlobalHelper.RequestEncodedParam("stripeapipublickey");
                string vpnsshpassword = Helpers.GlobalHelper.RequestEncodedParam("vpnsshpassword");
                string vpnsshuser = Helpers.GlobalHelper.RequestEncodedParam("vpnsshuser");

                string currency = Helpers.GlobalHelper.RequestEncodedParam("currency");
                string stripeplanid = Helpers.GlobalHelper.RequestEncodedParam("stripeplanid");

                decimal montlypaymentrate = decimal.Parse(Helpers.GlobalHelper.RequestEncodedParam("monthlypaymentrate"));
                decimal yearlypaymentrate = decimal.Parse(Helpers.GlobalHelper.RequestEncodedParam("yearlypaymentrate"));

                var info = new LibPoco.SiteInfo()
                {
                    Id = id,
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

                LibLogic.Helpers.SiteInfo.InitializeSimple(info, montlypaymentrate, yearlypaymentrate);
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
