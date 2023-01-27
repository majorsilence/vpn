using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Site.TestsFast.BetaSite;

/// <summary>
/// This class is called once for each namespace that has unit tests in it.
/// </summary>
[SetUpFixture()]
public class Setup
{
    private static readonly string testingdb = Guid.NewGuid().ToString().Replace("-", "");

    /// <summary>
    /// Called once before unit tests in a namespace are tested.  Only called once for all tests.
    /// </summary>
    [SetUp()]
    public void BringUp()
    {
        // setup database and shit
        var email = new Logic.Email.FakeEmail();
        var setup = new Logic.InitializeSettings("localhost", testingdb, email, false);
        setup.Execute();


        // set test server ssh port
        using (var db = Logic.InitializeSettings.DbFactory)
        {
            db.Open();
            var siteInfo = db.Query<Poco.SiteInfo>("SELECT * FROM SiteInfo");

            // See Vagrantfile vpnauthoritytest for ssh port number
            siteInfo.First().SshPort = 8023;
            // FIXME: Read api keys from appsettings.json
            siteInfo.First().StripeAPIPublicKey = "";
            siteInfo.First().StripeAPISecretKey = "";

            db.Update<Poco.SiteInfo>(siteInfo.First());


            db.Insert(new Poco.BetaKeys("abc1", false, false));
            db.Insert(new Poco.BetaKeys("abc2", false, false));
            db.Insert(new Poco.BetaKeys("abc3", false, false));
            db.Insert(new Poco.BetaKeys("abc4", false, false));
            db.Insert(new Poco.BetaKeys("abc5", false, false));
        }
    }

    /// <summary>
    /// Called once after unit tests in a namespace are tested.  Only called once for all tests.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        var connStrDrop = Logic.InitializeSettings.DbFactoryWithoutDatabase.ConnectionString;
        var cnDrop = new MySql.Data.MySqlClient.MySqlConnection(connStrDrop);
        var cmdDrop = cnDrop.CreateCommand();
        cmdDrop.CommandText = string.Format("DROP DATABASE IF EXISTS `{0}`;", testingdb);
        cmdDrop.CommandType = CommandType.Text;

        cnDrop.Open();
        cmdDrop.ExecuteNonQuery();
        cnDrop.Close();
    }
}