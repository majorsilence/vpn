using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class ResetPasswordController : Controller
{
    public ActionResult Index(string email)
    {
        ViewData["email"] = email;
        return View();
    }
}