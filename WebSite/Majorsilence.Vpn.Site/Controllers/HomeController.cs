using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class HomeController : Controller
    {

        readonly ISessionVariables sessionInstance;
        public HomeController(ISessionVariables sessionInstance)
        {
            this.sessionInstance = sessionInstance;
        }

        public ActionResult Index(string betaemail, string betacode)
        {
            ViewData["betaemail"] = betaemail;
            ViewData["betacode"] = betacode;
            return View(new Models.CustomViewLayout(sessionInstance));
        }
    }
}
