using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VpnSite.Controllers
{
    public class AdminRegionsController : Controller
    {
        public ActionResult Index()
        {
            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return null;
            }

            var model = new Models.AdminRegions();
            return View(model);
        }
    }
}
