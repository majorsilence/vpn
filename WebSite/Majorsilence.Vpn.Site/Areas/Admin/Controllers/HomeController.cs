using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Areas.Admin.Controllers;

[Area("Admin")]
public class HomeController : Controller
{
    private readonly ISessionVariables sessionInstance;

    public HomeController(ISessionVariables sessionInstance)
    {
        this.sessionInstance = sessionInstance;
    }

    // GET: HomeController
    public ActionResult Index()
    {
        return View(new CustomViewLayout(sessionInstance));
    }

    // GET: HomeController/Details/5
    public ActionResult Details(int id)
    {
        return View();
    }

    // GET: HomeController/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: HomeController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    // GET: HomeController/Edit/5
    public ActionResult Edit(int id)
    {
        return View();
    }

    // POST: HomeController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    // GET: HomeController/Delete/5
    public ActionResult Delete(int id)
    {
        return View();
    }

    // POST: HomeController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }
}