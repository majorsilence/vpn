using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace VpnSite.Controllers
{
    public class AdminRegionsEditController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EditRegions()
        {
            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return null;
            }

            int n;
            bool isNumeric = int.TryParse(VpnSite.Helpers.GlobalHelper.RequestParam("id").Trim().ToLower(), out n);

            var edit = new LibLogic.Admin.Regions();

            string description = VpnSite.Helpers.GlobalHelper.RequestParam("desc").Trim().ToLower();

            string activeString = VpnSite.Helpers.GlobalHelper.RequestParam("active");
            bool active = false;
            if (activeString != null)
            {
                active = true;
            }

            if (isNumeric)
            {
                edit.Update(n, description, active);
            }
            else
            {
                edit.Insert(description, active);
            }


            return View();
        }
    }
}

