using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.AppSettings;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Helpers;

namespace Majorsilence.Vpn.BackgroundWorker;

public class DailyProcessingWorker : BackgroundService
{
    private IEmail email;
    private ILogger _logger;
    private string vpnConnectionString;

    public DailyProcessingWorker(ILogger logger, SmtpSettings smtp, IConfiguration configuration)
    {
        _logger = logger;
        email = new LiveEmail(smtp.FromAddress, smtp.Username, smtp.Password, smtp.Host, smtp.Port);
        vpnConnectionString = configuration.GetConnectionString("MySqlVpn");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                var proc = new DailyProcessing(_logger);
                proc.Execute();
                _logger.LogInformation("DailyProcessingWorker: Run");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DailyProcessingWorker: Failed running DailyProcessing");
            }
            await Task.Delay(1000 * 60 * 60 * 2, stoppingToken); // every 2 hours
        }
        
    }
}