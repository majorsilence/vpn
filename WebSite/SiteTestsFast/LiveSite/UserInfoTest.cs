using System;
using System.Linq;
using NUnit.Framework;
using Dapper;
using Dapper.Contrib.Extensions;

namespace SiteTestsFast.LiveSite
{
    public class UserInfoTest
    {

        private readonly string emailAddress = "testuserinfo@majorsilence.com";
        private readonly string unicodeEmailAddress = "ಠ_ಠabc@majorsilence.com";


        [TearDown()]
        public void Cleanup()
        {
            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();
                cn.Execute("DELETE FROM Users WHERE Email = @email", new {email = emailAddress});
                cn.Execute("DELETE FROM Users WHERE Email = @email", new {email = unicodeEmailAddress});

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
        public void RetreiveUserInfoTest()
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
                , false, LibLogic.Setup.Email);

            var userid = peterAccount.Execute();

            Assert.That(AccountExists(emailAddress), Is.True);


            var info = new LibLogic.Accounts.UserInfo(userid);
            var profile = info.GetProfile();

            Assert.That("Peter", Is.EqualTo(profile.FirstName));
            Assert.That("Gill", Is.EqualTo(profile.LastName));
            Assert.That(emailAddress, Is.EqualTo(profile.Email));
            Assert.That(false, Is.EqualTo(profile.Admin));
            Assert.That(false, Is.EqualTo(profile.IsBetaUser));


        }

        [Test()]
        public void UpdateUserInfoTest()
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
                , false, LibLogic.Setup.Email);

            var userid = peterAccount.Execute();

            Assert.That(AccountExists(emailAddress), Is.True);


            var info = new LibLogic.Accounts.UserInfo(userid);
            var profile = info.GetProfile();

            Assert.That("Peter", Is.EqualTo(profile.FirstName));
            Assert.That("Gill", Is.EqualTo(profile.LastName));
            Assert.That(emailAddress, Is.EqualTo(profile.Email));
            Assert.That(false, Is.EqualTo(profile.Admin));
            Assert.That(false, Is.EqualTo(profile.IsBetaUser));



            info.UpdateProfile(unicodeEmailAddress, "Happy", "Dude");
            Assert.That("Happy", Is.EqualTo(profile.FirstName));
            Assert.That("Dude", Is.EqualTo(profile.LastName));
            Assert.That(unicodeEmailAddress, Is.EqualTo(profile.Email));


            var info2 = new LibLogic.Accounts.UserInfo(userid);
            var profile2 = info2.GetProfile();
            Assert.That("Happy", Is.EqualTo(profile2.FirstName));
            Assert.That("Dude", Is.EqualTo(profile2.LastName));
            Assert.That(unicodeEmailAddress, Is.EqualTo(profile2.Email));

        }


        [Test()]
        [ExpectedException(typeof(LibLogic.Exceptions.InvalidDataException))]
        public void UpdateUserInfoInvalidFirstNameTest()
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
                , false, LibLogic.Setup.Email);

            var userid = peterAccount.Execute();

            Assert.That(AccountExists(emailAddress), Is.True);


            var info = new LibLogic.Accounts.UserInfo(userid);
           // var profile = info.GetProfile();

            info.UpdateProfile(unicodeEmailAddress, "", "Dude");
           


        }

        [Test()]
        [ExpectedException(typeof(LibLogic.Exceptions.InvalidDataException))]
        public void UpdateUserInfoInvalidLastNameTest()
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
                , false, LibLogic.Setup.Email);

            var userid = peterAccount.Execute();

            Assert.That(AccountExists(emailAddress), Is.True);


            var info = new LibLogic.Accounts.UserInfo(userid);
            var profile = info.GetProfile();

            info.UpdateProfile(unicodeEmailAddress, "Happy", "");



        }

        [Test()]
        [ExpectedException(typeof(LibLogic.Exceptions.InvalidDataException))]
        public void UpdateUserInfoInvalidEmailTest()
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
                , false, LibLogic.Setup.Email);

            var userid = peterAccount.Execute();

            Assert.That(AccountExists(emailAddress), Is.True);


            var info = new LibLogic.Accounts.UserInfo(userid);
            var profile = info.GetProfile();

            info.UpdateProfile("", "Happy", "Dude");



        }

        [Test()]
        [ExpectedException(typeof(LibLogic.Exceptions.EmailAddressAlreadyUsedException))]
        public void UpdateUserInfoEmailAddressAlreadyUsedTest()
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
                , false, LibLogic.Setup.Email);

            var userid = peterAccount.Execute();

            var peterAccount2 = new LibLogic.Accounts.CreateAccount(
                new LibLogic.Accounts.CreateAccountInfo()
            {
                Email = unicodeEmailAddress,
                EmailConfirm = unicodeEmailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password2",
                PasswordConfirm = "Password2",
                BetaKey = ""
            }
                , false, LibLogic.Setup.Email);

            peterAccount2.Execute();


            Assert.That(AccountExists(emailAddress), Is.True);


            var info = new LibLogic.Accounts.UserInfo(userid);
            var profile = info.GetProfile();

            info.UpdateProfile(unicodeEmailAddress, "Happy", "Dude");



        }

    }
}

