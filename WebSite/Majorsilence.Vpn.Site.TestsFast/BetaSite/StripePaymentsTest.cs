﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Dapper;
using Stripe;

namespace Majorsilence.Vpn.Site.TestsFast.BetaSite
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
            StripeConfiguration.SetApiKey(Majorsilence.Vpn.Logic.Helpers.SiteInfo.StripeAPISecretKey);

            Majorsilence.Vpn.Logic.Helpers.SslSecurity.Callback();

            var peterAccount = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
                                   new Majorsilence.Vpn.Logic.Accounts.CreateAccountInfo()
                {
                    Email = emailAddress,
                    EmailConfirm = emailAddress,
                    Firstname = "Peter",
                    Lastname = "Gill",
                    Password = "Password1",
                    PasswordConfirm = "Password1",
                    BetaKey = betaKey
                }
                , true, Majorsilence.Vpn.Logic.InitializeSettings.Email);

            this.userid = peterAccount.Execute();


            CreateToken();

        }

        private void CreateToken()
        {
            var myToken = new Stripe.TokenCreateOptions()
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
           
            var client = new StripeClient(Majorsilence.Vpn.Logic.Helpers.SiteInfo.StripeAPISecretKey);
            var tokenService = new Stripe.TokenService(client);
            Stripe.Token stripeToken = tokenService.Create(myToken);
            this.token = stripeToken.Id;
            System.Console.WriteLine("Token is: " + token);
        }

        [TearDown()]
        public void Cleanup()
        {
            token = "";

            using (var cn = Majorsilence.Vpn.Logic.InitializeSettings.DbFactory)
            {
                cn.Open();
                cn.Execute("DELETE FROM UserPayments");
                var userData = cn.Query<Majorsilence.Vpn.Poco.Users>("SELECT * FROM Users WHERE Email = @email", new {email = emailAddress});

                if (userData.First().StripeCustomerAccount.Trim() != "")
                {
                    var client = new StripeClient(Majorsilence.Vpn.Logic.Helpers.SiteInfo.StripeAPISecretKey);
                    var customerService = new Stripe.CustomerService(client);
                    customerService.Delete(userData.First().StripeCustomerAccount);
                }

                cn.Execute("DELETE FROM Users WHERE Email = @email", new {email = emailAddress});
                cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new {Code = betaKey});
            }
        }

        [Test()]
        public void HappyPathNoCouponTest()
        {
            Majorsilence.Vpn.Logic.Helpers.SslSecurity.Callback();
            var pay = new Majorsilence.Vpn.Logic.Payments.StripePayment(userid, new Majorsilence.Vpn.Logic.Email.FakeEmail());
            pay.MakePayment(this.token, "");
        }

        [Test()]
        public void MissingTokenTest()
        {
            Majorsilence.Vpn.Logic.Helpers.SslSecurity.Callback();
            var pay = new Majorsilence.Vpn.Logic.Payments.StripePayment(userid, new Majorsilence.Vpn.Logic.Email.FakeEmail());

            Assert.Throws<Majorsilence.Vpn.Logic.Exceptions.InvalidStripeTokenException>(() => pay.MakePayment("", ""));
           
        }


    }
}

