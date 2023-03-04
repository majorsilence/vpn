using System.Threading.Tasks;
using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class AdminInviteBetaUsersController : Controller
{
    private readonly ISessionVariables sessionInstance;

    public AdminInviteBetaUsersController(ISessionVariables sessionInstance)
    {
        this.sessionInstance = sessionInstance;
    }


    public ActionResult Index()
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return null;

        var model = new AdminInviteBetaUsers
        {
            SessionVariables = sessionInstance
        };
        return View(model);
    }

    public async Task<ActionResult> SendMail(string emailAddress)
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return null;

        var model = new AdminInviteBetaUsers
        {
            SessionVariables = sessionInstance
        };

        await model.SendMailAsync(emailAddress);
        return View(model);
    }
}