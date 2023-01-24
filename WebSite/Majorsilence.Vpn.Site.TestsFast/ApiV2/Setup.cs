using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using Moq;
using System.Collections.Specialized;
using Majorsilence.Vpn.Site.TestsFast.MvcFakes;

namespace Majorsilence.Vpn.Site.TestsFast.ApiV2
{
    /// <summary>
    /// This class is called once for each namespace that has unit tests in it.
    /// </summary>
    [SetUpFixture()]
    public class Setup
    {

        private static readonly string testingdb = Guid.NewGuid().ToString().Replace("-", "");

        public static string token1 { get; private set; }

        public static string token2 { get; private set; }

        public static readonly string emailAddress = "testlogins@majorsilence.com";
        public static readonly string betaKey = "abc1";
        public static readonly string password = "Password3";

        public static int userid { get; private set; }

        private void RetrieveLoginTokenAndAssert()
        {

            var peterAccount = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
                                   new Majorsilence.Vpn.Logic.Accounts.CreateAccountInfo()
                                   {
                                       Email = emailAddress,
                                       EmailConfirm = emailAddress,
                                       Firstname = "Peter",
                                       Lastname = "Gill",
                                       Password = password,
                                       PasswordConfirm = password,
                                       BetaKey = betaKey
                                   }
                , true, Majorsilence.Vpn.Logic.InitializeSettings.Email);

            userid = peterAccount.Execute();

            // in an actual desktop app this will need to be setup as static
            var cookieContainer = new System.Net.CookieContainer();

            using (var handler = new System.Net.Http.HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new System.Net.Http.HttpClient(handler))
            {

                var byteArray = System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", emailAddress, password));
                var headerAuth = new System.Net.Http.Headers.AuthenticationHeaderValue(
                                     "Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Authorization = headerAuth;

                var mock = new Mock<Majorsilence.Vpn.Site.Helpers.ISessionVariables>();
                mock.SetupAllProperties();

                Majorsilence.Vpn.Site.Helpers.ISessionVariables sessionVars = mock.Object;
                var controller = new Majorsilence.Vpn.Site.Controllers.ApiV2Controller(sessionVars);

                var header = new NameValueCollection();
                header.Add("Authorization", headerAuth.ToString());


                // See http://stephenwalther.com/archive/2008/07/01/asp-net-mvc-tip-12-faking-the-controller-context

                FakeControllerContext.SetContext(controller, header);


                var blah = controller.Auth();
                //controller.Auth();
                System.Console.WriteLine(blah.Content);


                // mock.VerifySet(framework => framework.IsAdmin = true);
                Assert.That(sessionVars.LoggedIn, Is.EqualTo(true));
                Assert.That(sessionVars.Username, Is.EqualTo(emailAddress));
                Assert.That(sessionVars.UserId, Is.EqualTo(userid));
                Assert.That(sessionVars.IsAdmin, Is.EqualTo(true));
                Assert.That(controller.Response.StatusCode, Is.EqualTo((int)System.Net.HttpStatusCode.OK));


                var content = Newtonsoft.Json.JsonConvert.DeserializeObject<Majorsilence.Vpn.Logic.DTO.ApiAuthResponse>(blah.Content);
                Assert.That(string.IsNullOrEmpty(content.Token1), Is.EqualTo(false));
                Assert.That(string.IsNullOrEmpty(content.Token2), Is.EqualTo(false));

                Assert.That(content.Token1ExpireUtc, Is.GreaterThan(DateTime.UtcNow));
                Assert.That(content.Token1ExpireUtc, Is.LessThan(DateTime.UtcNow.AddDays(1)));

                Assert.That(content.Token2ExpireUtc, Is.GreaterThan(DateTime.UtcNow.AddDays(1)));
                Assert.That(content.Token2ExpireUtc, Is.LessThan(DateTime.UtcNow.AddDays(2)));

                token1 = content.Token1;
                token2 = content.Token2;

                // See http://haacked.com/archive/2007/06/19/unit-tests-web-code-without-a-web-server-using-httpsimulator.aspx/

                // var responseString = response.Content.ReadAsStringAsync();


            }
        }

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
                // FIXME: Read api keys from appsettings.json
                siteInfo.First().StripeAPIPublicKey = "";
                siteInfo.First().StripeAPISecretKey = "";

                db.Update<Majorsilence.Vpn.Poco.SiteInfo>(siteInfo.First());


                db.Insert(new Majorsilence.Vpn.Poco.BetaKeys("abc1", false, false));
                db.Insert(new Majorsilence.Vpn.Poco.BetaKeys("abc2", false, false));
                db.Insert(new Majorsilence.Vpn.Poco.BetaKeys("abc3", false, false));
                db.Insert(new Majorsilence.Vpn.Poco.BetaKeys("abc4", false, false));
                db.Insert(new Majorsilence.Vpn.Poco.BetaKeys("abc5", false, false));

            }

            RetrieveLoginTokenAndAssert();
        }

        private void CleanLogin()
        {
            using (var cn = Majorsilence.Vpn.Logic.InitializeSettings.DbFactory)
            {
                cn.Open();
                cn.Execute("DELETE FROM UsersApiTokens WHERE UserId = @Id", new { Id = userid });
                cn.Execute("DELETE FROM Users WHERE Email = @email", new { email = emailAddress });
                cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new { Code = betaKey });
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

