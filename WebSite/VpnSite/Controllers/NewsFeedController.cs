using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class NewsFeedController : Controller
    {
        public ActionResult Index()
        {
            return View ();
        }
    }
}
