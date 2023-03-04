using Majorsilence.Vpn.BackgroundWorker;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Payments;
using MySql.Data.MySqlClient;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((h,services) =>
    {
        services.AddTransient(_ =>
            new MySqlConnection(h.Configuration["ConnectionStrings:LocalMySqlServer"]));

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
        services.AddHostedService<DatabaseMigrationWorker>();
    })
    .Build();

await host.RunAsync();