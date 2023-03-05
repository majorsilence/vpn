using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.MySql;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Poco;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Majorsilence.Vpn.Logic;

public class DataMigrations
{
    private readonly DatabaseSettings _dbSettings;

    public DataMigrations(DatabaseSettings dbSettings)
    {
        _dbSettings = dbSettings;
    }


    public async Task ExecuteAsync()
    {
        CreateIfNotExists();
        MigrateDatabase();
        await CreateTestAccount();
    }

    private void MigrateDatabase()
    {
        // See http://stackoverflow.com/questions/7574417/is-it-possible-to-use-fluent-migrator-in-application-start

        // var announcer = new NullAnnouncer();
        var announcer =
            new TextWriterAnnouncer(s => Debug.WriteLine(s));
        var assembly = Assembly.GetExecutingAssembly();

        var migrationContext = new RunnerContext(announcer)
        {
            Namespace = "Majorsilence.Vpn.Logic.Migrations",
            TransactionPerSession = true
        };

        var options = new ProcessorOptions
        {
            PreviewOnly = false, // set to true to see the SQL
            Timeout = TimeSpan.FromSeconds(60)
        };

        var factory = new MySql5ProcessorFactory();
        var processor = factory.Create(_dbSettings.DbFactory.ConnectionString, announcer, options);
        var runner = new MigrationRunner(assembly, migrationContext, processor);
        runner.MigrateUp(true);
    }

    /// <summary>
    ///     Create the dabase if it does not exist.
    /// </summary>
    /// <remarks>Mysql/mariadb specific</remarks>
    private void CreateIfNotExists()
    {
        // return new MySql.Data.MySqlClient.MySqlConnection(string.Format("Server={0};Uid={1};Pwd={2};Port={3};CharSet=utf8mb4;",
        //   server, username, password, port));

        var x = new MySqlConnectionStringBuilder(_dbSettings.StrConnectionVpn);
        var serverConnectionVpn = $"server={x.Server};user={x.UserID};pwd={x.Password};Port={x.Port};CharSet=utf8mb4;";

        using (var cn = new MySqlConnection(serverConnectionVpn))
        {
            using var cmd = cn.CreateCommand();
            cmd.CommandText =
                $"CREATE DATABASE IF NOT EXISTS `{x.Database}` CHARACTER SET utf8 COLLATE utf8_unicode_ci;";

            cmd.CommandType = CommandType.Text;

            cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();
        }

        // Sessions database
        // Alls sessions are store in mysql instead of in process.
        // Provides better support for clustering, load balancing and restarting sites after updates
        // without users being logged out or loosing data

        var y = new MySqlConnectionStringBuilder(_dbSettings.StrConnectionSessions);
        var serverConnectionSession =
            $"server={y.Server};user={y.UserID};pwd={y.Password};Port={y.Port};CharSet=utf8mb4;";


        using (var cn2 = new MySqlConnection(serverConnectionSession))
        {
            using var cmd2 = cn2.CreateCommand();
            cmd2.CommandText =
                $"CREATE DATABASE IF NOT EXISTS `{y.Database}` CHARACTER SET utf8 COLLATE utf8_unicode_ci;";
            cmd2.CommandType = CommandType.Text;

            cn2.Open();
            cmd2.ExecuteNonQuery();
            cn2.Close();
        }
    }

    private async Task CreateTestAccount()
    {
        // TODO: Replace values with settings in appSetting.json

        using (var db = _dbSettings.DbFactory)
        {
            db.Open();
            var peter = db.Query<Users>("SELECT * FROM Users WHERE Email = @Email",
                new { Email = "atestuser@majorsilence.com" });
            if (peter.Count() == 0)
            {
                var peterAccount = new CreateAccount(
                    new CreateAccountInfo
                    {
                        Email = "atestuser@majorsilence.com",
                        EmailConfirm = "atestuser@majorsilence.com",
                        Firstname = "A Test",
                        Lastname = "User",
                        Password = "Password1",
                        PasswordConfirm = "Password1",
                        BetaKey = "AbC56#"
                    }
                    , true, new FakeEmail());
                await peterAccount.ExecuteAsync();
            }
        }
    }
}