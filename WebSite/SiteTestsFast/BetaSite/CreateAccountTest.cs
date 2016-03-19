using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Dapper;

namespace SiteTestsFast.BetaSite
{
    public class CreateAccountTest
    {
        private readonly string emailAddress = "test@majorsilence.com";
        private readonly string unicodeEmailAddress = "ಠ_ಠ@majorsilence.com";
        private readonly string betaKey = "abc1";
        private readonly string betaKey2 = "abc2";

        [TearDown()]
        public void Cleanup()
        {
            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();
                cn.Execute("DELETE FROM Users WHERE Email = @email", new {email = emailAddress});
                cn.Execute("DELETE FROM Users WHERE Email = @email", new {email = unicodeEmailAddress});
                cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new {Code = betaKey});
                cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new {Code = betaKey2});
            }
        }

        private bool AccountExists(string email)
        {
            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();
                var users = cn.Query<LibPoco.Users>("SELECT * FROM Users WHERE Email = @Email", new {Email = email});
                return (users.Count() == 1);   
            }
       
        }

        [Test()]
        public void ValidDataTest()
        {

            Assert.That(AccountExists(emailAddress), Is.False);

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

            peterAccount.Execute();

            Assert.That(AccountExists(emailAddress), Is.True);
        }

        [Test()]
        [ExpectedException (typeof(LibLogic.Exceptions.EmailAddressAlreadyUsedException))]
        public void DuplicateEmailTest()
        {

            Assert.That(AccountExists(emailAddress), Is.False);

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
                , false, LibLogic.Setup.Email);

            peterAccount.Execute();


            var peterAccount2 = new LibLogic.Accounts.CreateAccount(
                new LibLogic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey2
            }
                , false, LibLogic.Setup.Email);
            peterAccount2.Execute();

            Assert.That(AccountExists(emailAddress), Is.True);
        }
            
        [Test()]
        [ExpectedException (typeof(LibLogic.Exceptions.PasswordMismatchException))]
        public void PasswordMismatch()
        {
            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new LibLogic.Accounts.CreateAccount(
                new LibLogic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "A different password",
                BetaKey = betaKey
            }
                , true, LibLogic.Setup.Email);

            peterAccount.Execute();


        }

        [Test()]
        [ExpectedException (typeof(LibLogic.Exceptions.PasswordLengthException))]
        public void PasswordLengthTest()
        {
            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new LibLogic.Accounts.CreateAccount(
                new LibLogic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "",
                PasswordConfirm = "",
                BetaKey = betaKey
            }
                , true, LibLogic.Setup.Email);

            peterAccount.Execute();


        }

        [Test()]
        [ExpectedException (typeof(LibLogic.Exceptions.EmailMismatchException))]
        public void EmailMismatch()
        {
            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new LibLogic.Accounts.CreateAccount(
                new LibLogic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = "AveryDefadfasdemail@majorsilence.com",
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey
            }
                , true, LibLogic.Setup.Email);

            peterAccount.Execute();


        }

        [Test()]
        public void TestUnicode()
        {

            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new LibLogic.Accounts.CreateAccount(
                new LibLogic.Accounts.CreateAccountInfo()
            {
                Email = unicodeEmailAddress,
                EmailConfirm = unicodeEmailAddress,
                Firstname = "§",
                Lastname = "¼",
                Password = "β",
                PasswordConfirm = "β",
                BetaKey = betaKey
            }
                , true, LibLogic.Setup.Email);

            peterAccount.Execute();

            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();
                var data = cn.Query<LibPoco.Users>("SELECT * FROM Users WHERE Email = @email", new {email = unicodeEmailAddress});

                Assert.That(data.First().IsBetaUser, Is.True);
                Assert.That(data.First().Admin, Is.True);
                Assert.That(data.First().FirstName, Is.EqualTo("§"));
                Assert.That(data.First().LastName, Is.EqualTo("¼"));

            }

        }

        [Test()]
        public void IsAdmin()
        {
            Assert.That(AccountExists(emailAddress), Is.False);

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

            peterAccount.Execute();

            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();
                var data = cn.Query<LibPoco.Users>("SELECT * FROM Users WHERE Email = @email", new {email = emailAddress});

                Assert.That(data.First().Admin, Is.True);

            }

        }
            
        [Test()]
        public void IsNotAdmin()
        {
            Assert.That(AccountExists(emailAddress), Is.False);

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
                , false, LibLogic.Setup.Email);

            peterAccount.Execute();

            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();
                var data = cn.Query<LibPoco.Users>("SELECT * FROM Users WHERE Email = @email", new {email = emailAddress});

                Assert.That(data.First().Admin, Is.False);

            }

        }

        [Test()]
        [ExpectedException (typeof(LibLogic.Exceptions.InvalidDataException))]
        public void FirstNameMissing()
        {
            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new LibLogic.Accounts.CreateAccount(
                new LibLogic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey
            }
                , true, LibLogic.Setup.Email);

            peterAccount.Execute();


        }

        [Test()]
        [ExpectedException (typeof(LibLogic.Exceptions.InvalidDataException))]
        public void LastNameMissing()
        {
            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new LibLogic.Accounts.CreateAccount(
                new LibLogic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey
            }
                , true, LibLogic.Setup.Email);

            peterAccount.Execute();


        }

        [Test()]
        [ExpectedException (typeof(LibLogic.Exceptions.InvalidDataException))]
        public void EmailAddressMissing()
        {
            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new LibLogic.Accounts.CreateAccount(
                new LibLogic.Accounts.CreateAccountInfo()
            {
                Email = "",
                EmailConfirm = "",
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey
            }
                , true, LibLogic.Setup.Email);

            peterAccount.Execute();


        }

        [Test()]
        [ExpectedException (typeof(LibLogic.Exceptions.BetaKeyAlreadyUsedException))]
        public void BetaKeyAlreadyInUse()
        {

            Assert.That(AccountExists(emailAddress), Is.False);

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

            peterAccount.Execute();

            var peterAccount2 = new LibLogic.Accounts.CreateAccount(
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

            peterAccount2.Execute();

            Assert.That(AccountExists(emailAddress), Is.True);
        }


        [Test()]
        [ExpectedException (typeof(LibLogic.Exceptions.InvalidBetaKeyException))]
        public void InvalidBetaKey()
        {

            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new LibLogic.Accounts.CreateAccount(
                new LibLogic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = "A Fake Beta Key"
            }
                , true, LibLogic.Setup.Email);

            peterAccount.Execute();


            Assert.That(AccountExists(emailAddress), Is.False);
        }

        [Test()]
        [ExpectedException (typeof(LibLogic.Exceptions.InvalidBetaKeyException))]
        public void EmptyBetaKey()
        {

            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new LibLogic.Accounts.CreateAccount(
                new LibLogic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = ""
            }
                , true, LibLogic.Setup.Email);

            peterAccount.Execute();


            Assert.That(AccountExists(emailAddress), Is.False);
        }
    }
}
