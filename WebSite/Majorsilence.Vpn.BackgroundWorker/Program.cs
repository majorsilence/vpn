using Majorsilence.Vpn.BackgroundWorker;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Payments;
using MySql.Data.MySqlClient;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((h,services) =>
    {
        services.AddScoped<Majorsilence.Vpn.Logic.AppSettings.Settings>(i =>
        {
            return h.Configuration.GetSection("Settings").Get<Majorsilence.Vpn.Logic.AppSettings.Settings>();
        });
        services.AddScoped<IEmail>(i =>
        {
            var s = i.GetService<Majorsilence.Vpn.Logic.AppSettings.Settings>().Smtp;
            return new LiveEmail(s.FromAddress, s.Username, s.Password, s.Host, s.Port);
        });
        services.AddScoped<IPaypalSettings>(i => i.GetService<Majorsilence.Vpn.Logic.AppSettings.Settings>().Paypal);
        services.AddScoped<IEncryptionKeysSettings>(i => i.GetService<Majorsilence.Vpn.Logic.AppSettings.Settings>().EncryptionKeys);
        services.AddScoped<DatabaseSettings>(i => new DatabaseSettings(h.Configuration["ConnectionStrings:LocalMySqlServer"],
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