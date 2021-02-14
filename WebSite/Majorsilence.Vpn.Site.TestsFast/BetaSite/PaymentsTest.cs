using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Dapper;

namespace Majorsilence.Vpn.Site.TestsFast.BetaSite
{
    public class PaymentsTest
    {
        private readonly string emailAddress = "testpayments@majorsilence.com";
        private readonly string nonAdminEmail = "sdfasdf@majorsilence.com";


        private readonly string betaKey = "abc1";
        private readonly string betaKey2 = "abc2";
        private int userid;
        private int nonAdminUserId;

        [SetUp()]
        public void Setup()
        {

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
                , true, Majorsilence.Vpn.Logic.Setup.Email);

            this.userid = peterAccount.Execute();

            var account2 = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
                               new Majorsilence.Vpn.Logic.Accounts.CreateAccountInfo()
                {
                    Email = nonAdminEmail,
                    EmailConfirm = nonAdminEmail,
                    Firstname = "Peter",
                    Lastname = "Gill",
                    Password = "Password1",
                    PasswordConfirm = "Password1",
                    BetaKey = betaKey2
                }
                , false, Majorsilence.Vpn.Logic.Setup.Email);

            this.nonAdminUserId = account2.Execute();

        }

        [TearDown()]
        public void Cleanup()
        {
            using (var cn = Majorsilence.Vpn.Logic.Setup.DbFactory)
            {
                cn.Open();
                cn.Execute("DELETE FROM UserPayments");
                cn.Execute("DELETE FROM Users WHERE Email = @email", new {email = emailAddress});
                cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new {Code = betaKey});

                cn.Execute("DELETE FROM Users WHERE Email = @email", new {email = nonAdminEmail});
                cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new {Code = betaKey2});
            }
        }


        [Test()]
        public void ValidPaymentTest()
        {
            var createDate = DateTime.UtcNow;
            const decimal payment = 56.76m;
            var paycode = Majorsilence.Vpn.Logic.Helpers.SiteInfo.MonthlyPaymentId;

            var pay = new Majorsilence.Vpn.Logic.Payments.Payment(this.userid);
            pay.SaveUserPayment(payment, createDate, paycode);

            using (var cn = Majorsilence.Vpn.Logic.Setup.DbFactory)
            {
                cn.Open();
                var data = cn.Query<Majorsilence.Vpn.Poco.UserPayments>("SELECT * FROM UserPayments WHERE UserId = @UserId AND AmountPaid=@amount AND " +
                           "CreateTime=@CreateTime AND LookupPaymentTypeId=@payid", 
                               new {UserId = userid, amount = payment, CreateTime = createDate, payid = paycode});


                Assert.That(data.Count(), Is.EqualTo(1));
  
            }

        }

        [Test()]
        public void PaymentWithInvalidUserIdTest()
        {
            var createDate = DateTime.UtcNow;
            const decimal payment = 56.76m;
            var paycode = Majorsilence.Vpn.Logic.Helpers.SiteInfo.MonthlyPaymentId;

            var pay = new Majorsilence.Vpn.Logic.Payments.Payment(-1);

            Assert.Throws<Majorsilence.Vpn.Logic.Exceptions.InvalidUserIdException>(() => pay.SaveUserPayment(payment, createDate, paycode));
            

   

        }


        [Test()]
        public void IsExpiredTest()
        {
            // Beta account should never be expired
            var pay = new Majorsilence.Vpn.Logic.Payments.Payment(this.nonAdminUserId);
            Assert.That(pay.IsExpired(), Is.True);

        }

        [Test()]
        public void IsAdminExpiredTest()
        {
            // Beta account should never be expired
            var pay = new Majorsilence.Vpn.Logic.Payments.Payment(this.userid);
            Assert.That(pay.IsExpired(), Is.False);

        }



        [Test()]
        public void IsExpiredOldPaymentsTest()
        {

            var createDate = DateTime.UtcNow.AddMonths(-3);
            const decimal payment = 56.76m;
            var paycode = Majorsilence.Vpn.Logic.Helpers.SiteInfo.MonthlyPaymentId;

            var pay = new Majorsilence.Vpn.Logic.Payments.Payment(this.nonAdminUserId);
            pay.SaveUserPayment(payment, createDate, paycode);


            Assert.That(pay.IsExpired(), Is.True);

        }

        [Test()]
        public void IsAdminExpiredOldPaymentsTest()
        {

            var createDate = DateTime.UtcNow.AddMonths(-3);
            var payment = 56.76m;
            var paycode = Majorsilence.Vpn.Logic.Helpers.SiteInfo.MonthlyPaymentId;

            var pay = new Majorsilence.Vpn.Logic.Payments.Payment(this.userid);
            pay.SaveUserPayment(payment, createDate, paycode);


            Assert.That(pay.IsExpired(), Is.False);

        }


        [Test()]
        public void IsExpiredNewPaymentsTest()
        {
            // Beta account should never be expired
            var createDate = DateTime.UtcNow;
            var payment = 56.76m;
            var paycode = Majorsilence.Vpn.Logic.Helpers.SiteInfo.MonthlyPaymentId;

            var pay = new Majorsilence.Vpn.Logic.Payments.Payment(this.userid);
            pay.SaveUserPayment(payment, createDate, paycode);


            Assert.That(pay.IsExpired(), Is.False);

        }

        [Test()]
        public void IsExpiredInvalidUserIdTest()
        {
            // Beta account should never be expired except for invalid user accounts
            var pay = new Majorsilence.Vpn.Logic.Payments.Payment(-1);

            Assert.That(pay.IsExpired(), Is.True);

        }


        [Test()]
        public void HistoryTest()
        {
            var createDate = DateTime.UtcNow;
            var payment = 56.76m;
            var paycode = Majorsilence.Vpn.Logic.Helpers.SiteInfo.MonthlyPaymentId;

            var pay = new Majorsilence.Vpn.Logic.Payments.Payment(this.userid);


            for (int i = 0; i < 1000; i++)
            {
                pay.SaveUserPayment(payment, createDate.AddMonths(i), paycode);
                Assert.That(pay.History().Count(), Is.EqualTo(i + 1));
            }
                
        }

        [Test()]
        public void ExpireMonthlyPaymentTest()
        {
            var createDate = DateTime.UtcNow;
            var payment = 56.76m;
            var paycode = Majorsilence.Vpn.Logic.Helpers.SiteInfo.MonthlyPaymentId;

            var pay = new Majorsilence.Vpn.Logic.Payments.Payment(this.userid);

            pay.SaveUserPayment(payment, createDate, paycode);
            DateTime expireDate = createDate.AddMonths(1).AddDays(1);
            Assert.That(pay.ExpireDate().Year, Is.EqualTo(expireDate.Year));
            Assert.That(pay.ExpireDate().Month, Is.EqualTo(expireDate.Month));
            Assert.That(pay.ExpireDate().Day, Is.EqualTo(expireDate.Day));
            Assert.That(pay.ExpireDate().Hour, Is.EqualTo(expireDate.Hour));
            Assert.That(pay.ExpireDate().Minute, Is.EqualTo(expireDate.Minute));
            Assert.That(pay.ExpireDate().Second, Is.EqualTo(expireDate.Second));
           

        }

        [Test()]
        public void ExpireYearlyPaymentTest()
        {
            var createDate = DateTime.UtcNow;
            var payment = 56.76m;
            var paycode = Majorsilence.Vpn.Logic.Helpers.SiteInfo.YearlyPaymentId;

            var pay = new Majorsilence.Vpn.Logic.Payments.Payment(this.userid);

            pay.SaveUserPayment(payment, createDate, paycode);
            DateTime expireDate = createDate.AddYears(1).AddDays(1);
            Assert.That(pay.ExpireDate().Year, Is.EqualTo(expireDate.Year));
            Assert.That(pay.ExpireDate().Month, Is.EqualTo(expireDate.Month));
            Assert.That(pay.ExpireDate().Day, Is.EqualTo(expireDate.Day));
            Assert.That(pay.ExpireDate().Hour, Is.EqualTo(expireDate.Hour));
            Assert.That(pay.ExpireDate().Minute, Is.EqualTo(expireDate.Minute));
            Assert.That(pay.ExpireDate().Second, Is.EqualTo(expireDate.Second));


        }


    }
}

