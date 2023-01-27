using System.IO;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class KnowledgeBaseController : Controller
{
    private readonly IWebHostEnvironment _env;

    public KnowledgeBaseController(IWebHostEnvironment env)
    {
        _env = env;
    }

    public ActionResult Index(string code)
    {
        var path = Path.Combine(_env.ContentRootPath, "~/assets");
        var model = new KnowledgeBase(code, path);
        return View(model);
    }
}