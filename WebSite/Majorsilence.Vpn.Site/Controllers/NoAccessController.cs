using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class NoAccessController : Controller
{
    public ActionResult Index()
    {
        return View();
    }
}