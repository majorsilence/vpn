using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VpnSite.Controllers
{
    public class PricingController : Controller
    {
        public ActionResult Index()
        {
            var model = new Models.Pricing();
            return View (model);
        }
    }
}
