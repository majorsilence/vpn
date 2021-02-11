using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class AdminInviteBetaUsersController : Controller
    {

        readonly ISessionVariables sessionInstance;
        public AdminInviteBetaUsersController(ISessionVariables sessionInstance)
        {
            this.sessionInstance = sessionInstance;
        }


        public ActionResult Index()
        {
            if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false)
            {
                return null;
            }

            var model = new Models.AdminInviteBetaUsers();
            return View(model);
        }

        public ActionResult SendMail(string emailAddress)
        {
            if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false)
            {
                return null;
            }

            var model = new Models.AdminInviteBetaUsers();

            model.SendMail(emailAddress);
            return View(model);
        }
    }
}

