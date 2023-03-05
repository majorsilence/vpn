using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.AppSettings;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Helpers;

namespace Majorsilence.Vpn.BackgroundWorker;

public class DailyProcessingWorker : BackgroundService
{
    private readonly IEmail _email;
    private readonly ILogger _logger;
    private readonly DatabaseSettings _dbSettings;
    private readonly ActionLog _actionLog;
    public DailyProcessingWorker(ILogger logger, IEmail email, DatabaseSettings dbSettings,
        ActionLog actionLog)
    {
        _logger = logger;
        _email = email;
        _dbSettings = dbSettings;
        _actionLog = actionLog;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var proc = new DailyProcessing(_logger, _dbSettings, _actionLog);
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