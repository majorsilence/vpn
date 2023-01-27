using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class ServicesController : Controller
{
    private readonly ISessionVariables sessionInstance;

    public ServicesController(ISessionVariables sessionInstance)
    {
        this.sessionInstance = sessionInstance;
    }

    public ActionResult Index()
    {
        return View(new CustomViewLayout(sessionInstance));
    }
}