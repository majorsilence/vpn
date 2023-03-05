using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Admin;
using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class AdminRegionsEditController : Controller
{
    private readonly ISessionVariables sessionInstance;
    private readonly DatabaseSettings _dbSettings;

    public AdminRegionsEditController(ISessionVariables sessionInstance,
        DatabaseSettings dbSettings)
    {
        this.sessionInstance = sessionInstance;
        _dbSettings = dbSettings;
    }


    public ActionResult Index(int? id, string desc, string active)
    {
        ViewData["id"] = id;
        ViewData["desc"] = desc;
        ViewData["active"] = active;
        return View(new CustomViewLayout(sessionInstance));
    }

    public ActionResult EditRegions(int? id, string desc, string active)
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return null;

        int n;

        var edit = new Regions(_dbSettings);


        var activeYes = false;
        if (active != null) activeYes = true;

        if (id.HasValue)
            edit.Update(id.Value, desc, activeYes);
        else
            edit.Insert(desc, activeYes);


        return View(new CustomViewLayout(sessionInstance));
    }
}