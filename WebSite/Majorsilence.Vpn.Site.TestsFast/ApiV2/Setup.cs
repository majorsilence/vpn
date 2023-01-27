﻿using System;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.DTO;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Poco;
using Majorsilence.Vpn.Site.Controllers;
using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.TestsFast.MvcFakes;
using Moq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using NUnit.Framework;
using BetaKeys = Majorsilence.Vpn.Poco.BetaKeys;

namespace Majorsilence.Vpn.Site.TestsFast.ApiV2;

/// <summary>
///     This class is called once for each namespace that has unit tests in it.
/// </summary>
[SetUpFixture]
public class Setup
{
    private static readonly string testingdb = Guid.NewGuid().ToString().Replace("-", "");

    public static readonly string emailAddress = "testlogins@majorsilence.com";
    public static readonly string betaKey = "abc1";
    public static readonly string password = "Password3";

    public static string token1 { get; private set; }

    public static string token2 { get; private set; }

    public static int userid { get; private set; }

    private void RetrieveLoginTokenAndAssert()
    {
        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = password,
                PasswordConfirm = password,
                BetaKey = betaKey
            }
            , true, InitializeSettings.Email);

        userid = peterAccount.Execute();

        // in an actual desktop app this will need to be setup as static
        var cookieContainer = new CookieContainer();

        using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
        using (var client = new HttpClient(handler))
        {
            var byteArray = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", emailAddress, password));
            var headerAuth = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Authorization = headerAuth;

            var mock = new Mock<ISessionVariables>();
            mock.SetupAllProperties();

            var sessionVars = mock.Object;
            var controller = new ApiV2Controller(sessionVars);

            var header = new NameValueCollection();
            header.Add("Authorization", headerAuth.ToString());


            // See http://stephenwalther.com/archive/2008/07/01/asp-net-mvc-tip-12-faking-the-controller-context

            FakeControllerContext.SetContext(controller, header);


            var blah = controller.Auth();
            //controller.Auth();
            Console.WriteLine(blah.Content);


            // mock.VerifySet(framework => framework.IsAdmin = true);
            Assert.That(sessionVars.LoggedIn, Is.EqualTo(true));
            Assert.That(sessionVars.Username, Is.EqualTo(emailAddress));
            Assert.That(sessionVars.UserId, Is.EqualTo(userid));
            Assert.That(sessionVars.IsAdmin, Is.EqualTo(true));
            Assert.That(controller.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));


            var content = JsonConvert.DeserializeObject<ApiAuthResponse>(blah.Content);
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
    ///     Called once before unit tests in a namespace are tested.  Only called once for all tests.
    /// </summary>
    [SetUp]
    public void BringUp()
    {
        // setup database and shit
        var email = new FakeEmail();
        var setup = new InitializeSettings("localhost", testingdb, email, false);
        setup.Execute();


        // set test server ssh port
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            var siteInfo = db.Query<SiteInfo>("SELECT * FROM SiteInfo");

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

        RetrieveLoginTokenAndAssert();
    }

    private void CleanLogin()
    {
        using (var cn = InitializeSettings.DbFactory)
        {
            cn.Open();
            cn.Execute("DELETE FROM UsersApiTokens WHERE UserId = @Id", new { Id = userid });
            cn.Execute("DELETE FROM Users WHERE Email = @email", new { email = emailAddress });
            cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new { Code = betaKey });
        }
    }

    /// <summary>
    ///     Called once after unit tests in a namespace are tested.  Only called once for all tests.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        var connStrDrop = InitializeSettings.DbFactoryWithoutDatabase.ConnectionString;
        var cnDrop = new MySqlConnection(connStrDrop);
        var cmdDrop = cnDrop.CreateCommand();
        cmdDrop.CommandText = string.Format("DROP DATABASE IF EXISTS `{0}`;", testingdb);
        cmdDrop.CommandType = CommandType.Text;

        cnDrop.Open();
        cmdDrop.ExecuteNonQuery();
        cnDrop.Close();
    }
}