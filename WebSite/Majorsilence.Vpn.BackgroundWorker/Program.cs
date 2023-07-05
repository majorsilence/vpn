using Majorsilence.Vpn.BackgroundWorker;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.AppSettings;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Payments;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((h, services) =>
    {
        services.AddScoped<Settings>(i => { return h.Configuration.GetSection("Settings").Get<Settings>(); });
        services.AddScoped<IEmail>(i =>
        {
            var s = i.GetService<Settings>().Smtp;
            return new LiveEmail(s.FromAddress, s.Username, s.Password, s.Host, s.Port);
        });
        services.AddScoped<IPaypalSettings>(i => i.GetService<Settings>().Paypal);
        services.AddScoped<IEncryptionKeysSettings>(i => i.GetService<Settings>().EncryptionKeys);
        services.AddScoped<DatabaseSettings>(i => new DatabaseSettings(h.Configuration["ConnectionStrings:MySqlVpn"],
            h.Configuration["ConnectionStrings:MySqlSessions"],
            false,
            i.GetService<ILogger>()
        ));
        services.AddScoped<ActionLog>();
        services.AddHostedService<DatabaseMigrationWorker>();
        services.AddHostedService<StartupWorker>();
        services.AddHostedService<DailyProcessingWorker>();
        services.AddHostedService<EmailWorker>();
    })
    .Build();

await host.RunAsync();