﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Dapper;

namespace Majorsilence.Vpn.Site.TestsFast.BetaSite
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
            using (var cn = Majorsilence.Vpn.Logic.InitializeSettings.DbFactory)
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
            using (var cn = Majorsilence.Vpn.Logic.InitializeSettings.DbFactory)
            {
                cn.Open();
                var users = cn.Query<Majorsilence.Vpn.Poco.Users>("SELECT * FROM Users WHERE Email = @Email", new {Email = email});
                return (users.Count() == 1);   
            }
       
        }

        [Test()]
        public void ValidDataTest()
        {

            Assert.That(AccountExists(emailAddress), Is.False);

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

            peterAccount.Execute();

            Assert.That(AccountExists(emailAddress), Is.True);
        }

        [Test()]
        public void DuplicateEmailTest()
        {

            Assert.That(AccountExists(emailAddress), Is.False);

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
                , false, Majorsilence.Vpn.Logic.InitializeSettings.Email);

            peterAccount.Execute();


            var peterAccount2 = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
                new Majorsilence.Vpn.Logic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey2
            }
                , false, Majorsilence.Vpn.Logic.InitializeSettings.Email);

            Assert.Throws<Majorsilence.Vpn.Logic.Exceptions.EmailAddressAlreadyUsedException>(() => peterAccount2.Execute());
   
            Assert.That(AccountExists(emailAddress), Is.True);
        }
            
        [Test()]
        public void PasswordMismatch()
        {
            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
                new Majorsilence.Vpn.Logic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "A different password",
                BetaKey = betaKey
            }
                , true, Majorsilence.Vpn.Logic.InitializeSettings.Email);

            Assert.Throws<Majorsilence.Vpn.Logic.Exceptions.PasswordMismatchException>(() => peterAccount.Execute());
           

        }

        [Test()]
        public void PasswordLengthTest()
        {
            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
                new Majorsilence.Vpn.Logic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "",
                PasswordConfirm = "",
                BetaKey = betaKey
            }
                , true, Majorsilence.Vpn.Logic.InitializeSettings.Email);


            Assert.Throws<Majorsilence.Vpn.Logic.Exceptions.PasswordLengthException>(() => peterAccount.Execute());
          


        }

        [Test()]
        public void EmailMismatch()
        {
            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
                new Majorsilence.Vpn.Logic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = "AveryDefadfasdemail@majorsilence.com",
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey
            }
                , true, Majorsilence.Vpn.Logic.InitializeSettings.Email);

            Assert.Throws<Majorsilence.Vpn.Logic.Exceptions.EmailMismatchException>(() => peterAccount.Execute());
            


        }

        [Test()]
        public void TestUnicode()
        {

            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
                new Majorsilence.Vpn.Logic.Accounts.CreateAccountInfo()
            {
                Email = unicodeEmailAddress,
                EmailConfirm = unicodeEmailAddress,
                Firstname = "§",
                Lastname = "¼",
                Password = "β",
                PasswordConfirm = "β",
                BetaKey = betaKey
            }
                , true, Majorsilence.Vpn.Logic.InitializeSettings.Email);

            peterAccount.Execute();

            using (var cn = Majorsilence.Vpn.Logic.InitializeSettings.DbFactory)
            {
                cn.Open();
                var data = cn.Query<Majorsilence.Vpn.Poco.Users>("SELECT * FROM Users WHERE Email = @email", new {email = unicodeEmailAddress});

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

            peterAccount.Execute();

            using (var cn = Majorsilence.Vpn.Logic.InitializeSettings.DbFactory)
            {
                cn.Open();
                var data = cn.Query<Majorsilence.Vpn.Poco.Users>("SELECT * FROM Users WHERE Email = @email", new {email = emailAddress});

                Assert.That(data.First().Admin, Is.True);

            }

        }
            
        [Test()]
        public void IsNotAdmin()
        {
            Assert.That(AccountExists(emailAddress), Is.False);

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
                , false, Majorsilence.Vpn.Logic.InitializeSettings.Email);

            peterAccount.Execute();

            using (var cn = Majorsilence.Vpn.Logic.InitializeSettings.DbFactory)
            {
                cn.Open();
                var data = cn.Query<Majorsilence.Vpn.Poco.Users>("SELECT * FROM Users WHERE Email = @email", new {email = emailAddress});

                Assert.That(data.First().Admin, Is.False);

            }

        }

        [Test()]
        public void FirstNameMissing()
        {
            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
                new Majorsilence.Vpn.Logic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey
            }
                , true, Majorsilence.Vpn.Logic.InitializeSettings.Email);

            Assert.Throws<Majorsilence.Vpn.Logic.Exceptions.InvalidDataException>(() => peterAccount.Execute());
           


        }

        [Test()]
        public void LastNameMissing()
        {
            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
                new Majorsilence.Vpn.Logic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey
            }
                , true, Majorsilence.Vpn.Logic.InitializeSettings.Email);

            Assert.Throws<Majorsilence.Vpn.Logic.Exceptions.InvalidDataException>(() => peterAccount.Execute());
           


        }

        [Test()]
        public void EmailAddressMissing()
        {
            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
                new Majorsilence.Vpn.Logic.Accounts.CreateAccountInfo()
            {
                Email = "",
                EmailConfirm = "",
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey
            }
                , true, Majorsilence.Vpn.Logic.InitializeSettings.Email);

            Assert.Throws<Majorsilence.Vpn.Logic.Exceptions.InvalidDataException>(() => peterAccount.Execute());
            


        }

        [Test()]
        public void BetaKeyAlreadyInUse()
        {

            Assert.That(AccountExists(emailAddress), Is.False);

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

            peterAccount.Execute();

            var peterAccount2 = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
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

            Assert.Throws<Majorsilence.Vpn.Logic.Exceptions.BetaKeyAlreadyUsedException>(() => peterAccount2.Execute());
            

            Assert.That(AccountExists(emailAddress), Is.True);
        }


        [Test()]
        public void InvalidBetaKey()
        {

            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
                new Majorsilence.Vpn.Logic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = "A Fake Beta Key"
            }
                , true, Majorsilence.Vpn.Logic.InitializeSettings.Email);


            Assert.Throws<Majorsilence.Vpn.Logic.Exceptions.InvalidBetaKeyException>(() => peterAccount.Execute());
            


            Assert.That(AccountExists(emailAddress), Is.False);
        }

        [Test()]
        public void EmptyBetaKey()
        {

            Assert.That(AccountExists(emailAddress), Is.False);

            var peterAccount = new Majorsilence.Vpn.Logic.Accounts.CreateAccount(
                new Majorsilence.Vpn.Logic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = ""
            }
                , true, Majorsilence.Vpn.Logic.InitializeSettings.Email);

            Assert.Throws<Majorsilence.Vpn.Logic.Exceptions.InvalidBetaKeyException>(() => peterAccount.Execute());
            


            Assert.That(AccountExists(emailAddress), Is.False);
        }
    }
}
