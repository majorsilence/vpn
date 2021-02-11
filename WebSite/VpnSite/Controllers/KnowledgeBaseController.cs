using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class KnowledgeBaseController : Controller
    {
        public ActionResult Index(string code)
        {
            var model = new Models.KnowledgeBase(code);
            return View(model);
        }
    }
}
