using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.AppSettings;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Helpers;

namespace Majorsilence.Vpn.BackgroundWorker;

public class DatabaseMigrationWorker : BackgroundService
{
    private IEmail email;
    private ILogger _logger;
    private string vpnConnectionString;
    string sessionConnectionString;

    public DatabaseMigrationWorker(ILogger logger, SmtpSettings smtp, IConfiguration configuration)
    {
        _logger = logger;
        email = new LiveEmail(smtp.FromAddress, smtp.Username, smtp.Password, smtp.Host, smtp.Port);
        vpnConnectionString = configuration.GetConnectionString("MySqlVpn");
        sessionConnectionString = configuration.GetConnectionString("MySqlSessions");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var setup = new InitializeSettings(vpnConnectionString,
            sessionConnectionString,
            email,
            false,
            _logger);

        try
        {
            await Retry.DoAsync(async () => { await setup.ExecuteAsync(); }, TimeSpan.FromSeconds(2), 5);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DatabaseMigrationWorker: It appears the database migrations failed");
        }
    }
}