using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class AccountCreatedController : Controller
{
    public ActionResult Index(string status)
    {
        ViewData["status"] = status;
        return View();
    }
}