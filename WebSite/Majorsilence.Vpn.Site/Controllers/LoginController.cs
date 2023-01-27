using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Majorsilence.Vpn.Site.Controllers;

public class LoginController : Controller
{
    private readonly IStringLocalizer<LoginController> localizer;

    public LoginController(IStringLocalizer<LoginController> localizer)
    {
        this.localizer = localizer;
    }


    public ActionResult Index()
    {
        return View();
    }
}