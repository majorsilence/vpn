﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
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
