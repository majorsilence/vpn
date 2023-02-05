using System;
using System.Threading;
using System.Threading.Tasks;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VpnSite.Models;

namespace Majorsilence.Vpn.Site;

public class BackgroundWorker : BackgroundService
{
    private IEmail email;
    private ILogger _logger;
    private string vpnConnectionString;
    string sessionConnectionString;

    public BackgroundWorker(ILogger logger, SmtpSettings smtp, IConfiguration configuration)
    {
        _logger = logger;
        email = new LiveEmail(smtp.FromAddress, smtp.Username, smtp.Password, smtp.Host, smtp.Port);
        vpnConnectionString = configuration.GetConnectionString("MySqlVpn");
        sessionConnectionString = configuration.GetConnectionString("MySqlSessions");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var setup = new InitializeSettings(vpnConnectionString,
            sessionConnectionString,
            email,
            false,
            _logger);

        try
        {
            Retry.Do(() => { setup.Execute(); }, TimeSpan.FromSeconds(2), 5);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BackgroundWorker");
            email.SendMail_BackgroundThread("It appears the server setup failed: " + ex.Message,
                "MajorsilnceVPN setup failure on application_start", SiteInfo.AdminEmail, false);
        }

        return Task.CompletedTask;
    }
}