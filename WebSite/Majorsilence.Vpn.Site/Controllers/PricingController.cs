using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class PricingController : Controller
{
    public ActionResult Index()
    {
        var model = new Pricing();
        return View(model);
    }
}