using System;
using Majorsilence.Vpn.Logic.Admin;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class AdminVpnServersEditController : Controller
{
    private readonly ISessionVariables sessionInstance;

    public AdminVpnServersEditController(ISessionVariables sessionInstance)
    {
        this.sessionInstance = sessionInstance;
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

            var vpns = new VpnServers();
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
            Logging.Log(ex);
        }

        return View(new CustomViewLayout(sessionInstance));
    }
}