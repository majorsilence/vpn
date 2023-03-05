using System.Threading.Tasks;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class InviteController : Controller
{
    private readonly ISessionVariables sessionInstance;

    public InviteController(ISessionVariables sessionInstance)
    {
        this.sessionInstance = sessionInstance;
    }


    public ActionResult Index()
    {
        return View();
    }

    public async Task<ActionResult> SendMail(string emailladdress)
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return null;

        var keys = new BetaKeys(DatabaseSettings.Email);
        await keys.MailInvite(emailladdress, sessionInstance.UserId);

        return View();
    }
}