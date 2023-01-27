using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Dapper;
using Stripe;

namespace Majorsilence.Vpn.Site.TestsFast.BetaSite;

public class StripePaymentsTest
{
    public StripePaymentsTest()
    {
    }

    private readonly string emailAddress = "teststripepayments@majorsilence.com";
    private readonly string betaKey = "abc1";
    private int userid;
    private string token;

    [SetUp()]
    public void Setup()
    {
        StripeConfiguration.SetApiKey(Logic.Helpers.SiteInfo.StripeAPISecretKey);

        Logic.Helpers.SslSecurity.Callback();

        var peterAccount = new Logic.Accounts.CreateAccount(
            new Logic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey
            }
            , true, Logic.InitializeSettings.Email);

        userid = peterAccount.Execute();


        CreateToken();
    }

    private void CreateToken()
    {
        var myToken = new TokenCreateOptions()
        {
            Card = new TokenCardOptions()
            {
                AddressCountry = "US",
                AddressLine1 = "24 Portal St",
                AddressLine2 = "Unit B",
                AddressCity = "Biggie Smalls",
                AddressState = "NC",
                AddressZip = "27617",
                Cvc = "1223",
                ExpMonth = "10",
                ExpYear = "2030",
                Name = "Test Name",
                Number = "4242424242424242"
            }
        };

        // set these properties if using a card

        var client = new StripeClient(Logic.Helpers.SiteInfo.StripeAPISecretKey);
        var tokenService = new TokenService(client);
        var stripeToken = tokenService.Create(myToken);
        token = stripeToken.Id;
        Console.WriteLine("Token is: " + token);
    }

    [TearDown()]
    public void Cleanup()
    {
        token = "";

        using (var cn = Logic.InitializeSettings.DbFactory)
        {
            cn.Open();
            cn.Execute("DELETE FROM UserPayments");
            var userData =
                cn.Query<Poco.Users>("SELECT * FROM Users WHERE Email = @email", new { email = emailAddress });

            if (userData.First().StripeCustomerAccount.Trim() != "")
            {
                var client = new StripeClient(Logic.Helpers.SiteInfo.StripeAPISecretKey);
                var customerService = new CustomerService(client);
                customerService.Delete(userData.First().StripeCustomerAccount);
            }

            cn.Execute("DELETE FROM Users WHERE Email = @email", new { email = emailAddress });
            cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new { Code = betaKey });
        }
    }

    [Test()]
    public void HappyPathNoCouponTest()
    {
        Logic.Helpers.SslSecurity.Callback();
        var pay = new Logic.Payments.StripePayment(userid, new Logic.Email.FakeEmail());
        pay.MakePayment(token, "");
    }

    [Test()]
    public void MissingTokenTest()
    {
        Logic.Helpers.SslSecurity.Callback();
        var pay = new Logic.Payments.StripePayment(userid, new Logic.Email.FakeEmail());

        Assert.Throws<Logic.Exceptions.InvalidStripeTokenException>(() => pay.MakePayment("", ""));
    }
}