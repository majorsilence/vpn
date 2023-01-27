using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class AboutController : Controller
{
    public ActionResult Index()
    {
        return View();
    }
}