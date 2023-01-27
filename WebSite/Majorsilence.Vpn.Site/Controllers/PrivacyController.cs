using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class PrivacyController : Controller
{
    public ActionResult Index()
    {
        var model = new Privacy();
        return View(model);
    }
}