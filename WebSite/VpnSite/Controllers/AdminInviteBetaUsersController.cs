using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace VpnSite.Controllers
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

        public ActionResult SendMail()
        {
            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return null;
            }

            var model = new Models.AdminInviteBetaUsers();
            var emailAddress = VpnSite.Helpers.GlobalHelper.RequestParam("emailladdress").Trim();

            model.SendMail(emailAddress);
            return View(model);
        }
    }
}

