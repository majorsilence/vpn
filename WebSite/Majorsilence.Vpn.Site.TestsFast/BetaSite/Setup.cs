using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Site.TestsFast.BetaSite
{
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
            var email = new Majorsilence.Vpn.Logic.Email.FakeEmail();
            var setup = new Majorsilence.Vpn.Logic.InitializeSettings("localhost", testingdb, email, false);
            setup.Execute();


            // set test server ssh port
            using (var db = Majorsilence.Vpn.Logic.InitializeSettings.DbFactory)
            {
                db.Open();
                var siteInfo = db.Query<Majorsilence.Vpn.Poco.SiteInfo>("SELECT * FROM SiteInfo");

                // See Vagrantfile vpnauthoritytest for ssh port number
                siteInfo.First().SshPort = 8023;
                siteInfo.First().StripeAPIPublicKey = "pk_test_DBLlRp19zx2pnEYPgbPszWFr";
                siteInfo.First().StripeAPISecretKey = "sk_test_d2130qPEHAk9VNSXSX7fQFB9";

                db.Update<Majorsilence.Vpn.Poco.SiteInfo>(siteInfo.First());


                db.Insert(new Majorsilence.Vpn.Poco.BetaKeys("abc1", false, false));
                db.Insert(new Majorsilence.Vpn.Poco.BetaKeys("abc2", false, false));
                db.Insert(new Majorsilence.Vpn.Poco.BetaKeys("abc3", false, false));
                db.Insert(new Majorsilence.Vpn.Poco.BetaKeys("abc4", false, false));
                db.Insert(new Majorsilence.Vpn.Poco.BetaKeys("abc5", false, false));

            }

        }

        /// <summary>
        /// Called once after unit tests in a namespace are tested.  Only called once for all tests.
        /// </summary>
        [TearDown]
        public void TearDown()
        {

            string connStrDrop = Majorsilence.Vpn.Logic.InitializeSettings.DbFactoryWithoutDatabase.ConnectionString;
            var cnDrop = new MySql.Data.MySqlClient.MySqlConnection(connStrDrop);
            var cmdDrop = cnDrop.CreateCommand();
            cmdDrop.CommandText = string.Format("DROP DATABASE IF EXISTS `{0}`;", testingdb);
            cmdDrop.CommandType = System.Data.CommandType.Text;

            cnDrop.Open();
            cmdDrop.ExecuteNonQuery();
            cnDrop.Close();
        }
    }
}

