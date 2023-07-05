using Majorsilence.Vpn.Logic;

namespace Majorsilence.Vpn.BackgroundWorker;

public class DatabaseMigrationWorker : BackgroundService
{
    private readonly DatabaseSettings _dbSettings;
    private readonly ILogger _logger;

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