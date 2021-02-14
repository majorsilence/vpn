using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;

namespace SiteTestsFast.LiveSite
{
    /// <summary>
    /// This class is called once for each namespace that has unit tests in it.
    /// </summary>
    [SetUpFixture()]
    public class Setup
    {

        private static string testingdb = Guid.NewGuid().ToString().Replace("-", "");

        /// <summary>
        /// Called once before unit tests in a namespace are tested.  Only called once for all tests.
        /// </summary>
        [SetUp()]
        public void BringUp()
        {
        
            // setup database and shit
            var email = new LibLogic.Email.FakeEmail();
            var setup = new LibLogic.Setup("localhost", testingdb, email, true);
            setup.Execute();

            // set test server ssh port
            using (var db = LibLogic.Setup.DbFactory)
            {
                db.Open();
                var siteInfo = db.Query<LibPoco.SiteInfo>("SELECT * FROM SiteInfo");

                // See Vagrantfile vpnauthoritytest for ssh port number
                siteInfo.First().SshPort = 8023;
                siteInfo.First().StripeAPIPublicKey = "pk_test_DBLlRp19zx2pnEYPgbPszWFr";
                siteInfo.First().StripeAPISecretKey = "sk_test_d2130qPEHAk9VNSXSX7fQFB9";

                db.Update<LibPoco.SiteInfo>(siteInfo.First());


                db.Insert(new LibPoco.BetaKeys("abc1", false, false));
                db.Insert(new LibPoco.BetaKeys("abc2", false, false));
                db.Insert(new LibPoco.BetaKeys("abc3", false, false));
                db.Insert(new LibPoco.BetaKeys("abc4", false, false));
                db.Insert(new LibPoco.BetaKeys("abc5", false, false));

            }

      



        }

        /// <summary>
        /// Called once after unit tests in a namespace are tested.  Only called once for all tests.
        /// </summary>
        [TearDown]
        public void TearDown()
        {

            string connStrDrop = LibLogic.Setup.DbFactoryWithoutDatabase.ConnectionString;
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

