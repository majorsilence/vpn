using System;
using NUnit.Framework;
using Dapper;
using System.Linq;

namespace Majorsilence.Vpn.Site.TestsFast.BetaSite
{
    public class LoginNormalTest
    {
        private readonly string emailAddress = "testloginsnotadmin@majorsilence.com";
        private readonly string betaKey = "abc1";
        private readonly string password = "Password4";
        private int userid;

        [SetUp()]
        public void Setup()
        {

            var peterAccount = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
                                   new Majorsilence.Vpn.Logic.Accounts.CreateAccountInfo()
                {
                    Email = this.emailAddress,
                    EmailConfirm = this.emailAddress,
                    Firstname = "Peter",
                    Lastname = "Gill",
                    Password = this.password,
                    PasswordConfirm = this.password,
                    BetaKey = betaKey
                }
                , false, Majorsilence.Vpn.Logic.Setup.Email);

            peterAccount.Execute();
            System.Console.WriteLine("Created account");
            using (var cn = Majorsilence.Vpn.Logic.Setup.DbFactory)
            {
                cn.Open();
                var users = cn.Query<Majorsilence.Vpn.Poco.Users>("SELECT * FROM Users WHERE Email = @Email", new {Email = emailAddress});
                System.Console.WriteLine("user count " + users.Count());
                if (users.Count() == 1)
                {
                    userid = users.First().Id;
                }
                else
                {
                    throw new Majorsilence.Vpn.Logic.Exceptions.InvalidDataException("User for test not created");
                }
            }

        }

        [TearDown()]
        public void Cleanup()
        {
            using (var cn = Majorsilence.Vpn.Logic.Setup.DbFactory)
            {
                cn.Open();
                cn.Execute("DELETE FROM Users WHERE Email = @email", new {email = emailAddress});
                cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new {Code = betaKey});
            }
        }


        [Test()]
        public void CanLogin()
        {
            var login = new Majorsilence.Vpn.Logic.Login(emailAddress, this.password);
            login.Execute();

            System.Console.WriteLine(login.LoggedIn);
            Assert.That(login.LoggedIn, Is.True);

            System.Console.WriteLine(login.IsAdmin);
            Assert.That(login.IsAdmin, Is.False);

            System.Console.WriteLine(login.Username);
            Assert.That(login.Username, Is.EqualTo(emailAddress));

            System.Console.WriteLine(login.UserId);
            Assert.That(login.UserId, Is.EqualTo(this.userid));


        }

        [Test()]
        public void InvalidUsernameLogin()
        {
            var login = new Majorsilence.Vpn.Logic.Login("hithere", this.password);
            login.Execute();

            Assert.That(login.LoggedIn, Is.False);
            Assert.That(login.IsAdmin, Is.False);
            Assert.That(login.Username, Is.EqualTo("hithere"));
            Assert.That(login.UserId, Is.EqualTo(-1));


        }

        [Test()]
        public void InvalidPasswordLogin()
        {

            var login = new Majorsilence.Vpn.Logic.Login(this.emailAddress, "wrong password");
            login.Execute();

            Assert.That(login.LoggedIn, Is.False);
            Assert.That(login.IsAdmin, Is.False);
            Assert.That(login.Username, Is.EqualTo(this.emailAddress));
            Assert.That(login.UserId, Is.EqualTo(-1));


        }

        [Test()]
        public void InvalidUsernameAndPasswordLogin()
        {

            var login = new Majorsilence.Vpn.Logic.Login("hi there", "wrong password");
            login.Execute();

            Assert.That(login.LoggedIn, Is.False);
            Assert.That(login.IsAdmin, Is.False);
            Assert.That(login.Username, Is.EqualTo("hi there"));
            Assert.That(login.UserId, Is.EqualTo(-1));


        }
    }
}

