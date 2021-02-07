using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class AdminInviteBetaUsersController : Controller
    {

        public ActionResult Index()
        {
            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return null;
            }

            var model = new Models.AdminInviteBetaUsers();
            return View(model);
        }

        public ActionResult SendMail(string emailAddress)
        {
            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return null;
            }

            var model = new Models.AdminInviteBetaUsers();

            model.SendMail(emailAddress);
            return View(model);
        }
    }
}

