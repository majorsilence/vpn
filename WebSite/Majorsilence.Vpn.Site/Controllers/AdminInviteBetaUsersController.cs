using System.Threading.Tasks;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers;

public class AdminInviteBetaUsersController : Controller
{
    private readonly ISessionVariables sessionInstance;
    private readonly DatabaseSettings _dbSettings;
    private readonly IEmail _email;
    
    public AdminInviteBetaUsersController(ISessionVariables sessionInstance,
        DatabaseSettings dbSettings,
        IEmail email)
    {
        this.sessionInstance = sessionInstance;
        _dbSettings = dbSettings;
        _email = email;
    }


    public ActionResult Index()
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return null;

        var model = new AdminInviteBetaUsers(_email, _dbSettings)
        {
            SessionVariables = sessionInstance
        };
        return View(model);
    }

    public async Task<ActionResult> SendMail(string emailAddress)
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return null;

        var model = new AdminInviteBetaUsers(_email, _dbSettings)
        {
            SessionVariables = sessionInstance
        };

        await model.SendMailAsync(emailAddress);
        return View(model);
    }
}