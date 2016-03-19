using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VpnSite.Controllers
{
    public class PrivacyController : Controller
    {
        public ActionResult Index()
        {
            var model = new Models.Privacy();
            return View (model);
        }
    }
}
