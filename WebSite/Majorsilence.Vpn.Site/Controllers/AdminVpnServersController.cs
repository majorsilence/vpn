using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class AdminVpnServersController : Controller
{
    private readonly ISessionVariables sessionInstance;

    public AdminVpnServersController(ISessionVariables sessionInstance)
    {
        this.sessionInstance = sessionInstance;
    }

    public ActionResult Index()
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return null;

        var model = new Models.AdminVpnServers()
        {
            SessionVariables = sessionInstance
        };
        return View(model);
    }
}