using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Logic.Payments;
using Majorsilence.Vpn.Poco;
using NUnit.Framework;
using Stripe;
using SiteInfo = Majorsilence.Vpn.Logic.Helpers.SiteInfo;

namespace Majorsilence.Vpn.Site.TestsFast.BetaSite;

public class StripePaymentsTest
{
    private readonly string betaKey = "abc1";

    private readonly string emailAddress = "teststripepayments@majorsilence.com";
    private string token;
    private int userid;

    [SetUp]
    public async Task Setup()
    {
        StripeConfiguration.SetApiKey(SiteInfo.StripeAPISecretKey);

        SslSecurity.Callback();

        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey
            }
            , true, new FakeEmail());

        userid = await peterAccount.ExecuteAsync();


        CreateToken();
    }

    private void CreateToken()
    {
        var myToken = new TokenCreateOptions
        {
            Card = new TokenCardOptions
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

        var client = new StripeClient(SiteInfo.StripeAPISecretKey);
        var tokenService = new TokenService(client);
        var stripeToken = tokenService.Create(myToken);
        token = stripeToken.Id;
        Console.WriteLine("Token is: " + token);
    }

    [TearDown]
    public void Cleanup()
    {
        token = "";

        using (var cn = DatabaseSettings.DbFactory)
        {
            cn.Open();
            cn.Execute("DELETE FROM UserPayments");
            var userData =
                cn.Query<Users>("SELECT * FROM Users WHERE Email = @email", new { email = emailAddress });

            if (userData.First().StripeCustomerAccount.Trim() != "")
            {
                var client = new StripeClient(SiteInfo.StripeAPISecretKey);
                var customerService = new CustomerService(client);
                customerService.Delete(userData.First().StripeCustomerAccount);
            }

            cn.Execute("DELETE FROM Users WHERE Email = @email", new { email = emailAddress });
            cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new { Code = betaKey });
        }
    }

    [Test]
    public void HappyPathNoCouponTest()
    {
        SslSecurity.Callback();
        var pay = new StripePayment(userid, new FakeEmail());
        pay.MakePayment(token, "");
    }

    [Test]
    public void MissingTokenTest()
    {
        SslSecurity.Callback();
        var pay = new StripePayment(userid, new FakeEmail());

        Assert.Throws<InvalidStripeTokenException>(() => pay.MakePayment("", ""));
    }
}