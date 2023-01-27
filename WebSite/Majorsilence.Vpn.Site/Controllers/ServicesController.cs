using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class ServicesController : Controller
{
    private readonly ISessionVariables sessionInstance;

    public ServicesController(ISessionVariables sessionInstance)
    {
        this.sessionInstance = sessionInstance;
    }

    public ActionResult Index()
    {
        return View(new Models.CustomViewLayout(sessionInstance));
    }
}