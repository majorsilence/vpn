using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VpnSite.Controllers
{
    public class TermsController : Controller
    {
        public ActionResult Index()
        {
            var model = new Models.Terms();

            return View (model);
        }
    }
}
