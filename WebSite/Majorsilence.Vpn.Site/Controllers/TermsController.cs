using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class TermsController : Controller
{
    public ActionResult Index()
    {
        var model = new Terms();

        return View(model);
    }
}