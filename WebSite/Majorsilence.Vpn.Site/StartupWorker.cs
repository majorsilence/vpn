using System.Threading;
using System.Threading.Tasks;
using Majorsilence.Vpn.Logic;
using Microsoft.Extensions.Hosting;

namespace Majorsilence.Vpn.Site;

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