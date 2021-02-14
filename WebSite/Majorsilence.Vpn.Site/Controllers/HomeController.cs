using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string betaemail, string betacode)
        {
            ViewData["betaemail"] = betaemail;
            ViewData["betacode"] = betacode;
            return View ();
        }
    }
}
