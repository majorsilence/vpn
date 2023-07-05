using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class SetupController : Controller
{
    private readonly DatabaseSettings _dbSettings;
    private readonly ISessionVariables sessionInstance;

    public SetupController(ISessionVariables sessionInstance,
        DatabaseSettings dbSettings)
    {
        this.sessionInstance = sessionInstance;
        _dbSettings = dbSettings;
    }

    public ActionResult Index()
    {
        var model = new Setup(sessionInstance.UserId, sessionInstance.Username,
            _dbSettings)
        {
            SessionVariables = sessionInstance
        };
        return View(model);
    }
}