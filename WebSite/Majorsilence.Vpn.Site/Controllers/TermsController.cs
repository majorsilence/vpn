using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class TermsController : Controller
{
    private readonly DatabaseSettings _dbSettings;

    public TermsController(DatabaseSettings dbSettings)
    {
        _dbSettings = dbSettings;
    }

    public ActionResult Index()
    {
        var model = new Terms(_dbSettings);

        return View(model);
    }
}