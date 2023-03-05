using System;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Admin;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NuGet.Common;

namespace Majorsilence.Vpn.Site.Controllers;

public class AdminVpnServersEditController : Controller
{
    private readonly ISessionVariables sessionInstance;
    private readonly ILogger<AdminVpnServersEditController> _logger;
    private readonly DatabaseSettings _dbSettings;

    public AdminVpnServersEditController(ISessionVariables sessionInstance,
        ILogger<AdminVpnServersEditController> logger,
        DatabaseSettings dbSettings)
    {
        this.sessionInstance = sessionInstance;
        _logger = logger;
        _dbSettings = dbSettings;
    }

    public ActionResult Index(int? id, string address, int? port, string desc,
        int? region, string active)
    {
        ViewData["id"] = id;
        ViewData["address"] = address;
        ViewData["port"] = port;
        ViewData["desc"] = desc;
        ViewData["region"] = region;
        ViewData["active"] = active;
        return View(new CustomViewLayout(sessionInstance));
    }

    public ActionResult EditServers(int? id, string address, int port,
        string desc, int region, string active)
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return null;


        try
        {
            var activeYes = false;
            if (active != null) activeYes = true;

            var vpns = new VpnServers(_dbSettings);
            if (id.HasValue)
                vpns.Update(id.Value, address,
                    port,
                    desc,
                    region,
                    activeYes);
            else
                vpns.Insert(address,
                    port,
                    desc,
                    region,
                    activeYes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AdminVpnServersEditController");
        }

        return View(new CustomViewLayout(sessionInstance));
    }
}