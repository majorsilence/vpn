using System;
using System.Web.Mvc;

namespace VpnSite.Controllers
{
    public class InviteController : Controller
    {
        public InviteController()
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendMail()
        {
            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return null;
            }

            var emailAddress = VpnSite.Helpers.GlobalHelper.RequestParam("emailladdress").Trim();
            var keys = new LibLogic.Accounts.BetaKeys(LibLogic.Setup.Email);
            keys.MailInvite(emailAddress, Helpers.SessionVariables.Instance.UserId);

            return View();
        }

    }
}

