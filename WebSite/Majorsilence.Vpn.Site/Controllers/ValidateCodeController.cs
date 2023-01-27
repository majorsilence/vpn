using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class ValidateCodeController : Controller
{
    public ActionResult Index(string resetcode)
    {
        ViewData["resetcode"] = resetcode;
        return View();
    }
}