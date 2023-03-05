using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.AppSettings;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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