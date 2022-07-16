using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;

namespace SiteTestsFast
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
            var setup = new LibLogic.Setup("localhost", testingdb, true, email);
            setup.Execute();

            // set test server ssh port
            using (IDbConnection db = LibLogic.Setup.DbFactory) 
            {
                db.Open();
                var siteInfo = db.Query<LibPoco.SiteInfo> ("SELECT * FROM SiteInfo");

                // See Vagrantfile vpnauthoritytest for ssh port number
                siteInfo.First ().SshPort = 8023;

                db.Update<LibPoco.SiteInfo> (siteInfo.First ());


                db.Update(new LibPoco.BetaKeys("abc1", false));
                db.Update(new LibPoco.BetaKeys("abc2", false));
                db.Update(new LibPoco.BetaKeys("abc3", false));
                db.Update(new LibPoco.BetaKeys("abc4", false));
                db.Update(new LibPoco.BetaKeys("abc5", false));

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

