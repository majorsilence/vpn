using System;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
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

        public ActionResult SendMail(string emailladdress)
        {
            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return null;
            }

            var keys = new LibLogic.Accounts.BetaKeys(LibLogic.Setup.Email);
            keys.MailInvite(emailladdress, Helpers.SessionVariables.Instance.UserId);

            return View();
        }

    }
}

