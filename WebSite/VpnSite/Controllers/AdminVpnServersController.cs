using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace VpnSite.Controllers
{
    public class AdminVpnServersController : Controller
    {
        public ActionResult Index()
        {
            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return null;
            }

            var model = new Models.AdminVpnServers();
            return View(model);
        }
    }
}

