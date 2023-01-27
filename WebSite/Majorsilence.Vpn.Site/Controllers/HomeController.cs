using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class HomeController : Controller
{
    private readonly ISessionVariables sessionInstance;

    public HomeController(ISessionVariables sessionInstance)
    {
        this.sessionInstance = sessionInstance;
    }

    public ActionResult Index(string betaemail, string betacode)
    {
        ViewData["betaemail"] = betaemail;
        ViewData["betacode"] = betacode;
        return View(new CustomViewLayout(sessionInstance));
    }
}