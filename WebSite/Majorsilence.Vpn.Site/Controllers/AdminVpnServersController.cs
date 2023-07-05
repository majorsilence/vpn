using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class AdminVpnServersController : Controller
{
    private readonly DatabaseSettings _dbSettings;
    private readonly ISessionVariables sessionInstance;

    public AdminVpnServersController(ISessionVariables sessionInstance,
        DatabaseSettings dbSettings)
    {
        this.sessionInstance = sessionInstance;
        _dbSettings = dbSettings;
    }

    public ActionResult Index()
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return null;

        var model = new AdminVpnServers(_dbSettings)
        {
            SessionVariables = sessionInstance
        };
        return View(model);
    }
}