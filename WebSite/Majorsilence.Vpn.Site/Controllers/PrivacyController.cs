using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class PrivacyController : Controller
{
    private readonly DatabaseSettings _dbSettings;

    public PrivacyController(DatabaseSettings dbSettings)
    {
        _dbSettings = dbSettings;
    }

    public ActionResult Index()
    {
        var model = new Privacy(_dbSettings);
        return View(model);
    }
}