using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;

namespace Majorsilence.Vpn.Logic.Accounts
{
    public class CreateAccount
    {
        Email.IEmail email;

        private CreateAccount()
        {
        }

        private CreateAccountInfo details;
        private Majorsilence.Vpn.Poco.BetaKeys betaKey;

        public CreateAccount(CreateAccountInfo details, Email.IEmail email)
        {
            this.details = details;
            this.email = email;
        }

        private bool isAdmin = false;

        public CreateAccount(CreateAccountInfo details, bool isAdmin, Email.IEmail email)
        {
            this.details = details;
            this.isAdmin = isAdmin;
            this.email = email;
        }

        public int Execute()
        {
            ValidateData();

            using (var db = InitializeSettings.DbFactory)
            {
                db.Open();
               

                using (var txn = db.BeginTransaction())
                {

                    var pwd = new CreatePasswords(details.Password, details.Firstname + details.Lastname);

                    var userid = db.Insert(new Majorsilence.Vpn.Poco.Users(
                                     details.Email,
                                     pwd.Password,
                                     pwd.Salt,
                                     details.Firstname,
                                     details.Lastname,
                                     isAdmin,
                                     DateTime.UtcNow,
                                     "",
                                     "",
                                     "",
                         
                                     RequiresBetaKey()
                        

                                 ));

                    if (RequiresBetaKey())
                    {
                        betaKey.IsUsed = true;
                        db.Update(betaKey);
                    }

                    txn.Commit();

                    if (isAdmin == false)
                    {
                        EmailAccountCreation();
                    }

                    return (int)userid;
                }
            }

        }

        private void EmailAccountCreation()
        {
            string subject = string.Format("{0} Account Created", Helpers.SiteInfo.SiteName);
            var message = new System.Text.StringBuilder();

            message.Append(string.Format("Welcome to {0}", Helpers.SiteInfo.SiteName));
            message.Append("<br><br>");
            message.Append(string.Format("Your {0} account has been created.  ", Helpers.SiteInfo.SiteName));
            message.Append(string.Format("You can login and start using your account anytime at {0}."
                , Helpers.SiteInfo.SiteUrl));
                
            email.SendMail_BackgroundThread(message.ToString(), subject, this.details.Email, true, null);

        }

        private void BetaKeySetup()
        {
            using (var db = InitializeSettings.DbFactory)
            {
                db.Open();
                var info = db.Query<Majorsilence.Vpn.Poco.BetaKeys>("SELECT * FROM BetaKeys WHERE Code = @code", new { code = details.BetaKey });
                if (info.Count() != 1)
                {
                    throw new Exceptions.InvalidBetaKeyException("There appears to have been an error validating the beta key.");
                }

                betaKey = info.First();
                if (betaKey.IsUsed)
                {
                    throw new Exceptions.BetaKeyAlreadyUsedException("The beta key has already been used.");
                }
            }

        }

        private bool RequiresBetaKey()
        {
            if (Helpers.SiteInfo.LiveSite == false)
            {
                return true;
            }

            return false;
        }

        private void ValidateData()
        {

            if (details.Password != details.PasswordConfirm)
            {
                throw new Exceptions.PasswordMismatchException("Password and confirm password do not match");
            }

            if (details.Email != details.EmailConfirm)
            {
                throw new Exceptions.EmailMismatchException("Email and confirm email do not match");
            }

            if (details.Email.Trim() == "")
            {
                throw new Exceptions.InvalidDataException("An email address must be filled in");
            }

            if (details.Password.Trim() == "")
            {
                throw new Exceptions.PasswordLengthException("A password must be filled in");
            }

            if (details.Firstname.Trim() == "")
            {
                throw new Exceptions.InvalidDataException("A first name must be filled in");
            }

            if (details.Lastname.Trim() == "")
            {
                throw new Exceptions.InvalidDataException("A last name must be filled in");
            }

            if (RequiresBetaKey())
            {
                BetaKeySetup();
            }

            ValidateEmailNotAlreadyUsed();

        }

        private void ValidateEmailNotAlreadyUsed()
        {
            using (var db = InitializeSettings.DbFactory)
            {
                db.Open();
                var info = db.Query<Majorsilence.Vpn.Poco.BetaKeys>("SELECT * FROM Users WHERE Email = @Email", new { Email = details.Email });
                if (info.Count() > 0)
                {
                    throw new Exceptions.EmailAddressAlreadyUsedException("The email address specified for this account has already been used.");
                }
                    
            }
        }

    }
}