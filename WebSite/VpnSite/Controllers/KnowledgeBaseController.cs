using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class KnowledgeBaseController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public KnowledgeBaseController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public ActionResult Index(string code)
        {
            string path = System.IO.Path.Combine(_env.ContentRootPath, "~/assets");
            var model = new Models.KnowledgeBase(code, path);
            return View(model);
        }
    }
}
