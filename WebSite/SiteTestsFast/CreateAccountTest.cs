using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Dapper;

namespace SiteTestsFast
{
    public class CreateAccountTest
    {
        private readonly string emailAddress = "test@majorsilence.com";
        private readonly string betaKey = "abc1";

        [TearDown]
        public void Cleanup()
        {
            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();
                cn.Execute("DELETE FROM users WHERE email = @email", new {email = emailAddress});
                cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new {Code = betaKey});
            }
        }
            
        [Test()]
        public void BetaUserValidTest()
        {
        }

        [Test()]
        public void BetaUserInvalidTest()
        {

        }

        [Test()]
        public void BetaUserKeyAlreadyUsedTest()
        {
        }

        [Test()]
        public void RegularUserTest()
        {
            var details = new LibLogic.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                BetaKey = this.betaKey,
                Firstname = "hi there",
                Lastname = "hello world",
                Password = "password",
                PasswordConfirm = "password",

            };

            var email = new LibLogic.Email.FakeEmail();
            var account = new LibLogic.CreateAccount(details, email);
            account.Execute();


        }

        [Test()]
        public void RegularUserInvalidDataTest()
        {
        }
    }
}
