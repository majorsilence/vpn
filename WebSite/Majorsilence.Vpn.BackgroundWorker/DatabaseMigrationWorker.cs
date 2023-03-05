using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.AppSettings;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Helpers;

namespace Majorsilence.Vpn.BackgroundWorker;

public class DatabaseMigrationWorker : BackgroundService
{
    private readonly ILogger _logger;
    private readonly DatabaseSettings _dbSettings;
    
    public DatabaseMigrationWorker(ILogger logger, DatabaseSettings dbSettings)
    {
        _logger = logger;
        _dbSettings = dbSettings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var setup = new DataMigrations(_dbSettings);

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