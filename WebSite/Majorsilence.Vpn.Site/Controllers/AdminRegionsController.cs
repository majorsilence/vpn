using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class AdminRegionsController : Controller
{
    private readonly ISessionVariables sessionInstance;

    public AdminRegionsController(ISessionVariables sessionInstance)
    {
        this.sessionInstance = sessionInstance;
    }


    public ActionResult Index()
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return null;

        var model = new AdminRegions
        {
            SessionVariables = sessionInstance
        };
        return View(model);
    }
}