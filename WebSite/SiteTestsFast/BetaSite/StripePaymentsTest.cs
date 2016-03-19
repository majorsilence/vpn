using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Dapper;
using Stripe;

namespace SiteTestsFast.BetaSite
{
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
            StripeConfiguration.SetApiKey(LibLogic.Helpers.SiteInfo.StripeAPISecretKey);

            LibLogic.Helpers.SslSecurity.Callback();

            var peterAccount = new LibLogic.Accounts.CreateAccount(
                                   new LibLogic.Accounts.CreateAccountInfo()
                {
                    Email = emailAddress,
                    EmailConfirm = emailAddress,
                    Firstname = "Peter",
                    Lastname = "Gill",
                    Password = "Password1",
                    PasswordConfirm = "Password1",
                    BetaKey = betaKey
                }
                , true, LibLogic.Setup.Email);

            this.userid = peterAccount.Execute();


            CreateToken();

        }

        private void CreateToken()
        {
            var myToken = new StripeTokenCreateOptions();

            // set these properties if using a card
            myToken.CardAddressCountry = "US";
            myToken.CardAddressLine1 = "24 Portal St";
            myToken.CardAddressLine2 = "Unit B";
            myToken.CardAddressCity = "Biggie Smalls";
            myToken.CardAddressState = "NC";
            myToken.CardAddressZip = "27617";
            myToken.CardCvc = "1223";
            myToken.CardExpirationMonth = "10";
            myToken.CardExpirationYear = "2030";
            myToken.CardName = "Test Name";
            myToken.CardNumber = "4242424242424242";

            var tokenService = new StripeTokenService(LibLogic.Helpers.SiteInfo.StripeAPISecretKey);
            StripeToken stripeToken = tokenService.Create(myToken);
            this.token = stripeToken.Id;
            System.Console.WriteLine("Token is: " + token);
        }

        [TearDown()]
        public void Cleanup()
        {
            token = "";

            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();
                cn.Execute("DELETE FROM UserPayments");
                var userData = cn.Query<LibPoco.Users>("SELECT * FROM Users WHERE Email = @email", new {email = emailAddress});

                if (userData.First().StripeCustomerAccount.Trim() != "")
                {
                    var customerService = new StripeCustomerService(LibLogic.Helpers.SiteInfo.StripeAPISecretKey);
                    customerService.Delete(userData.First().StripeCustomerAccount);
                }

                cn.Execute("DELETE FROM Users WHERE Email = @email", new {email = emailAddress});
                cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new {Code = betaKey});
            }
        }

        [Test()]
        public void HappyPathNoCouponTest()
        {
            LibLogic.Helpers.SslSecurity.Callback();
            var pay = new LibLogic.Payments.StripePayment(userid, new LibLogic.Email.FakeEmail());
            pay.MakePayment(this.token, "");
        }

        [Test()]
        [ExpectedException(typeof(LibLogic.Exceptions.InvalidStripeTokenException))]
        public void MissingTokenTest()
        {
            LibLogic.Helpers.SslSecurity.Callback();
            var pay = new LibLogic.Payments.StripePayment(userid, new LibLogic.Email.FakeEmail());
            pay.MakePayment("", "");
        }


    }
}

