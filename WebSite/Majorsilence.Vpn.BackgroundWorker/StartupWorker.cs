using Majorsilence.Vpn.Logic;

namespace Majorsilence.Vpn.BackgroundWorker;

public class StartupWorker : BackgroundService
{
    private readonly DatabaseSettings _dbSettings;

    public StartupWorker(DatabaseSettings dbSettings)
    {
        _dbSettings = dbSettings;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var inst = new InitiateGlobalStaticVariables(_dbSettings);
        inst.Execute();
        return Task.CompletedTask;
    }
}