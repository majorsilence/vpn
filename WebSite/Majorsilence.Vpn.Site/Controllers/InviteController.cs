using System.Threading.Tasks;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class InviteController : Controller
{
    private readonly ISessionVariables sessionInstance;
    private readonly IEmail _email;
    private readonly DatabaseSettings _dbSettings;
    public InviteController(ISessionVariables sessionInstance,
        IEmail email,
        DatabaseSettings dbSettings)
    {
        this.sessionInstance = sessionInstance;
        _email = email;
        _dbSettings = dbSettings;
    }


    public ActionResult Index()
    {
        return View();
    }

    public async Task<ActionResult> SendMail(string emailladdress)
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return null;

        var keys = new BetaKeys(_email, _dbSettings);
        await keys.MailInvite(emailladdress, sessionInstance.UserId);

        return View();
    }
}