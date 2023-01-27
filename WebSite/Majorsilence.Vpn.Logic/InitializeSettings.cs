using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Timers;
using Dapper;
using Dapper.Contrib.Extensions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.MySql;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Poco;
using MySql.Data.MySqlClient;
using SiteInfo = Majorsilence.Vpn.Poco.SiteInfo;

namespace Majorsilence.Vpn.Logic;

public class InitializeSettings : ICommand
{
    private static string strConnectionVpn;
    private static string strConnectionSessions;
    private Timer dailyProcessingTimer;
    private readonly bool isLiveSite;

    private InitializeSettings()
    {
    }

    public InitializeSettings(string strVpnConnection, string sessionsConnection, IEmail email, bool isLiveSite)
    {
        strConnectionVpn = strVpnConnection;
        strConnectionSessions = sessionsConnection;

        Email = email;
        this.isLiveSite = isLiveSite;
    }

    public static IEmail Email { get; private set; }


    public static IDbConnection DbFactory => new MySqlConnection(strConnectionVpn);

    public static IDbConnection DbFactoryWithoutDatabase =>
        // return new MySql.Data.MySqlClient.MySqlConnection(string.Format("Server={0};Uid={1};Pwd={2};Port={3};CharSet=utf8mb4;",
        //   server, username, password, port));
        new MySqlConnection(strConnectionVpn);

    public void Execute()
    {
        CreateIfNotExists();
        MigrateDatabase();
        LoadCacheVariables();
        CreateTestAccount();
        InitializeTimers();
    }

    private void InitializeTimers()
    {
        const int dailyProcessInSeconds = 1000 * 60 * 60 * 2; // every 2 hours
        dailyProcessingTimer = new Timer(dailyProcessInSeconds);
        dailyProcessingTimer.Elapsed += DailyProcessing;
        dailyProcessingTimer.Enabled = true;
    }

    private void DailyProcessing(object sender, ElapsedEventArgs e)
    {
        try
        {
            var proc = new DailyProcessing();
            proc.Execute();
        }
        catch (Exception ex)
        {
            Logging.Log(ex, true);
        }
    }

    private void LoadCacheVariables()
    {
        LoadSiteInfo();
    }

    private void LoadSiteInfo()
    {
        using (var db = DbFactory)
        {
            db.Open();


            var data = db.Query<SiteInfo>("SELECT * FROM SiteInfo").ToList();
            if (data.Count == 0)
                throw new InvalidDataException("Invalid data in SiteInfo.  No data found.");
            if (data.Count > 1)
                throw new InvalidDataException("Invalid data in SiteInfo.  Multiple rows found.");

            data.First().LiveSite = isLiveSite;
            db.Update(data.First());

            var paymenttypes = LoadLookupPaymentTypes();
            var rates = LoadCurrentRates();

            Helpers.SiteInfo.Initialize(data.First(), paymenttypes.Item1, rates.Item1, rates.Item2,
                paymenttypes.Item2);
        }
    }

    private Tuple<int, int> LoadLookupPaymentTypes()
    {
        using (var db = DbFactory)
        {
            db.Open();

            var data = db.Query<LookupPaymentType>("SELECT * FROM LookupPaymentType");
            if (data.Count() == 0)
                throw new InvalidDataException("Invalid data in LoadLookupPaymentTypes.  No data found.");

            var monthly = 1;
            var yearly = 2;

            foreach (var x in data)
                if (x.Code == "MONTHLY")
                    monthly = x.Id;
                else if (x.Code == "YEARLY") yearly = x.Id;

            return Tuple.Create(monthly, yearly);
        }
    }

    private Tuple<decimal, decimal> LoadCurrentRates()
    {
        using (var db = DbFactory)
        {
            db.Open();

            var data = db.Query<PaymentRates>("SELECT * FROM PaymentRates");
            if (data.Count() == 0 || data.Count() > 1)
                throw new InvalidDataException("Invalid data in PaymentRates.  To many or to few rows");

            return Tuple.Create(data.First().CurrentMonthlyRate, data.First().CurrentYearlyRate);
        }
    }

    public void MigrateDatabase()
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
        var processor = factory.Create(DbFactory.ConnectionString, announcer, options);
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

        var x = new MySqlConnectionStringBuilder(strConnectionVpn);
        var serverConnectionVpn = $"server={x.Server};user={x.UserID};pwd={x.Password};Port={x.Port};CharSet=utf8mb4;";

        using (var cn = new MySqlConnection(serverConnectionVpn))
        {
            var cmd = cn.CreateCommand();
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

        var y = new MySqlConnectionStringBuilder(strConnectionSessions);
        var serverConnectionSession =
            $"server={y.Server};user={y.UserID};pwd={y.Password};Port={y.Port};CharSet=utf8mb4;";


        using (var cn2 = new MySqlConnection(serverConnectionSession))
        {
            var cmd2 = cn2.CreateCommand();
            cmd2.CommandText =
                $"CREATE DATABASE IF NOT EXISTS `{y.Database}` CHARACTER SET utf8 COLLATE utf8_unicode_ci;";
            cmd2.CommandType = CommandType.Text;

            cn2.Open();
            cmd2.ExecuteNonQuery();
            cn2.Close();
        }
    }

    private void CreateTestAccount()
    {
        // TODO: Replace values with settings in appSetting.json

        using (var db = DbFactory)
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
                    , true, Email);
                peterAccount.Execute();
            }
        }
    }
}