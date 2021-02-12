using System;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class InviteController : Controller
    {
        readonly ISessionVariables sessionInstance;
        public InviteController(ISessionVariables sessionInstance)
        {
            this.sessionInstance = sessionInstance;
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendMail(string emailladdress)
        {
            if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false)
            {
                return null;
            }

            var keys = new LibLogic.Accounts.BetaKeys(LibLogic.Setup.Email);
            keys.MailInvite(emailladdress, sessionInstance.UserId);

            return View();
        }

    }
}

