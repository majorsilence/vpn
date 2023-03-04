using System;
using System.Threading.Tasks;
using System.Web;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Logic.Payments;
using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SiteInfo = Majorsilence.Vpn.Poco.SiteInfo;

namespace Majorsilence.Vpn.Site.Controllers;

public class AdminController : Controller
{
    private readonly IEmail email;
    private readonly ISessionVariables sessionInstance;
    private ILogger _logger;
    public AdminController(IEmail email, ISessionVariables sessionInstance,
        ILogger logger)
    {
        this.email = email;
        this.sessionInstance = sessionInstance;
        _logger = logger;
    }


    public ActionResult Index()
    {
        return View(new CustomViewLayout(sessionInstance));
    }

    public ActionResult SiteInfo(string status)
    {
        ViewData["status"] = status;
        return View(new CustomViewLayout(sessionInstance));
    }

    public ActionResult Users(string status)
    {
        ViewData["status"] = status;

        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return null;

        var model = new Users
        {
            SessionVariables = sessionInstance
        };
        return View(model);
    }

    public ActionResult ErrorReport()
    {
        return View(new CustomViewLayout(sessionInstance));
    }

    public async Task RemoveStripeAccount(int id, string removeaccount)
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return;

        try
        {
            if (removeaccount != null && removeaccount == "yes")
            {
                var payments = new StripePayment(id, email);
                await payments.CancelSubscription();
                await payments.CancelAccount();


                Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode("User with id removed: " + id),
                    false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RemoveStripeAccount");
            Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode(ex.Message), false);
        }
    }

    [HttpPost]
    public async Task RemoveSubscription(int id, string removeaccount)
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return;

        try
        {
            if (removeaccount != null && removeaccount == "yes")
            {
                var payments = new StripePayment(id, email);
                await payments.CancelSubscription();


                Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode("User with id removed: " + id),
                    false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RemoveSubscription");
            Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode(ex.Message), false);
        }
    }

    [HttpPost]
    public void RemoveUser(int id, string removeaccount)
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return;

        try
        {
            if (removeaccount != null && removeaccount == "yes")
            {
                var user = new UserInfo(id, _logger);
                user.RemoveAccount();


                Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode("User with id removed: " + id),
                    false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RemoveUser");
            Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode(ex.Message), false);
        }
    }

    [HttpPost]
    public void ToggleAdmin(int id)
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return;

        try
        {
            var modify = new ModifyAccount();
            modify.ToggleIsAdmin(id);

            Response.Redirect(
                "/admin/users?status=" + HttpUtility.HtmlEncode("User with id admin mode was toggled: " + id), false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ToggleAdmin");
            Response.Redirect("/admin/users?status=" + HttpUtility.HtmlEncode(ex.Message), false);
        }
    }


    [HttpPost]
    public void SaveSiteInfo(int siteid, string adminemail, string livesite, string sitename, string siteurl,
        int sshport, string vpnsshuser, string vpnsshpassword, string stripeapipublickey,
        string stripeapisecretkey, string currency, string stripeplanid,
        decimal monthlypaymentrate, decimal yearlypaymentrate)
    {
        if (sessionInstance.LoggedIn == false || sessionInstance.IsAdmin == false) return;

        try
        {
            var islivesite = false;
            if (livesite == "on")
                // means no beta key required
                islivesite = true;

            var info = new SiteInfo
            {
                Id = siteid,
                AdminEmail = adminemail,
                LiveSite = islivesite,
                SiteName = sitename,
                SiteUrl = siteurl,
                SshPort = sshport,
                StripeAPISecretKey = stripeapisecretkey,
                StripeAPIPublicKey = stripeapipublickey,
                VpnSshPassword = vpnsshpassword,
                VpnSshUser = vpnsshuser,
                StripePlanId = stripeplanid,
                Currency = currency
            };

            Logic.Helpers.SiteInfo.InitializeSimple(info, monthlypaymentrate, yearlypaymentrate);
            Logic.Helpers.SiteInfo.SaveCurrentSettingsToDb();

            Response.Redirect("/admin/siteinfo?status=ok", false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SaveSiteInfo");
            Response.Redirect("/admin/siteinfo?status=" + HttpUtility.HtmlEncode(ex.Message), false);
        }
    }
}