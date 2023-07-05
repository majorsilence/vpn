using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Poco;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace SiteTests;

/// <summary>
///     This class is called once for each namespace that has unit tests in it.
/// </summary>
[SetUpFixture]
public class Setup
{
    private static string testingdb;

    public static DatabaseSettings DbSettings { get; private set; }
    public static IEmail EmailSettings { get; private set; }

    /// <summary>
    ///     Called once before unit tests in a namespace are tested.  Only called once for all tests.
    /// </summary>
    [SetUp]
    public async Task BringUp()
    {
        UpVpnTestServer();

        // setup database and stuff
        EmailSettings = new FakeEmail();
        var mockLogger = new Mock<ILogger>();
        var logger = mockLogger.Object;
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false);

        IConfiguration config = builder.Build();

        var cnBuilder = new MySqlConnectionStringBuilder(config["ConnectionStrings:MySqlVpn"]);
        testingdb = cnBuilder.Database;
        DbSettings = new DatabaseSettings(config["ConnectionStrings:MySqlVpn"],
            config["ConnectionStrings:MySqlSessions"],
            false, logger);
        var inst = new InitiateGlobalStaticVariables(DbSettings);
        inst.Execute();

        // set test server ssh port
        using (var db = DbSettings.DbFactory)
        {
            db.Open();
            var siteInfo = db.Query<SiteInfo>("SELECT * FROM SiteInfo");

            // See Vagrantfile vpnauthoritytest for ssh port number
            siteInfo.First().SshPort = 8023;

            db.Update(siteInfo.First());
        }
    }

    private void UpVpnTestServer()
    {
        using var p = new Process();
        p.StartInfo.FileName = "vagrant";
        p.StartInfo.Arguments = "up vpnauthoritytest";
        p.Start();
        p.WaitForExit();
    }

    /// <summary>
    ///     Called once after unit tests in a namespace are tested.  Only called once for all tests.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        var connStrDrop = DbSettings.DbFactoryWithoutDatabase.ConnectionString;
        using var cnDrop = new MySqlConnection(connStrDrop);
        using var cmdDrop = cnDrop.CreateCommand();
        cmdDrop.CommandText = string.Format("DROP DATABASE IF EXISTS `{0}`;", testingdb);
        cmdDrop.CommandType = CommandType.Text;

        cnDrop.Open();
        cmdDrop.ExecuteNonQuery();
        cnDrop.Close();


        DestroyVpnTestServer();
    }

    private void DestroyVpnTestServer()
    {
        using var p = new Process();
        p.StartInfo.FileName = "vagrant";
        p.StartInfo.Arguments = "destroy vpnauthoritytest -f";
        p.Start();
        p.WaitForExit();
    }
}