using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
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

        var model = new AdminVpnServers
        {
            SessionVariables = sessionInstance
        };
        return View(model);
    }
}