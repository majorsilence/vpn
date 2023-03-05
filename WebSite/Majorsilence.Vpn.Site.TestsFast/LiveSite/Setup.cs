using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Poco;
using Majorsilence.Vpn.Site.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace Majorsilence.Vpn.Site.TestsFast.LiveSite;

/// <summary>
///     This class is called once for each namespace that has unit tests in it.
/// </summary>
[SetUpFixture]
public class Setup
{
    private static readonly string testingdb = Guid.NewGuid().ToString().Replace("-", "");

    /// <summary>
    ///     Called once before unit tests in a namespace are tested.  Only called once for all tests.
    /// </summary>
    [SetUp]
    public async Task BringUp()
    {
        // setup database and stuff
        var email = new FakeEmail();
        var mockLogger = new Mock<ILogger>();
        var logger = mockLogger.Object;
        var setup = new DatabaseSettings("localhost", 
            testingdb, email, true,
            logger);
        await setup.ExecuteAsync();

        // set test server ssh port
        using (var db = DatabaseSettings.DbFactory)
        {
            db.Open();
            var siteInfo = db.Query<Poco.SiteInfo>("SELECT * FROM SiteInfo");

            // See Vagrantfile vpnauthoritytest for ssh port number
            siteInfo.First().SshPort = 8023;
            // FIXME: Read api keys from appsettings.json
            siteInfo.First().StripeAPIPublicKey = "";
            siteInfo.First().StripeAPISecretKey = "";

            db.Update(siteInfo.First());


            db.Insert(new BetaKeys("abc1", false, false));
            db.Insert(new BetaKeys("abc2", false, false));
            db.Insert(new BetaKeys("abc3", false, false));
            db.Insert(new BetaKeys("abc4", false, false));
            db.Insert(new BetaKeys("abc5", false, false));
        }
    }

    /// <summary>
    ///     Called once after unit tests in a namespace are tested.  Only called once for all tests.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        var connStrDrop = DatabaseSettings.DbFactoryWithoutDatabase.ConnectionString;
        var cnDrop = new MySqlConnection(connStrDrop);
        var cmdDrop = cnDrop.CreateCommand();
        cmdDrop.CommandText = string.Format("DROP DATABASE IF EXISTS `{0}`;", testingdb);
        cmdDrop.CommandType = CommandType.Text;

        cnDrop.Open();
        cmdDrop.ExecuteNonQuery();
        cnDrop.Close();
    }
}