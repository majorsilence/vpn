﻿using System;
using System.Linq;
using System.Collections.Generic;
using Dapper;
using Dapper.Contrib.Extensions;

namespace LibLogic.Accounts
{
    public class UserInfo
    {
        private LibPoco.Users details;

        private UserInfo()
        {
        }

        public static IEnumerable<LibPoco.Users> RetrieveUserList()
        {
            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();

                return cn.Query<LibPoco.Users>("SELECT * FROM Users;");

            }
        }

        public UserInfo(int userid)
        {

            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();

                details = cn.Get<LibPoco.Users>(userid);

            }

        }

        public void RemoveAccount()
        {
            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();

                using (var txn = cn.BeginTransaction())
                {

                    cn.Execute("DELETE FROM UserOpenVpnCerts WHERE UserId=@UserId", new {UserId = details.Id}, txn);
                    cn.Execute("DELETE FROM UserPayments WHERE UserId=@UserId", new {UserId = details.Id}, txn);
                    cn.Execute("DELETE FROM UserPptpInfo WHERE UserId=@UserId", new {UserId = details.Id}, txn);

                    cn.Delete(details, txn, null);
                    txn.Commit();
                }

            }
        }

        public void UpdatePassword(string oldPassword, string newPassword, string confirmNewPassword)
        {

            if (newPassword != confirmNewPassword)
            {
                throw new Exceptions.InvalidDataException("New password and confirm new password do not match.");
            }

            var login = new LibLogic.Login(details.Email, oldPassword);

            login.Execute();

            if (!login.LoggedIn)
            {
                throw new Exceptions.InvalidDataException("Invalid old password");
            }


            var pwd = new CreatePasswords(newPassword, details.FirstName + details.LastName);
            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();

                details.Password = pwd.Password;
                details.Salt = pwd.Salt;
               
                cn.Update(details);

            }

        }

        public void UpdateProfile(string email, string firstName, string lastName)
        {
            if (email.Trim() == "")
            {
                throw new Exceptions.InvalidDataException("An email address must be entered.");
            }
            else if (firstName.Trim() == "")
            {
                throw new Exceptions.InvalidDataException("A first name must be entered.");
            }
            else if (lastName.Trim() == "")
            {
                throw new Exceptions.InvalidDataException("A last name must be entered.");
            }
                
            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();

                if (email.Trim().ToLower() != details.Email.Trim().ToLower())
                {
                    var userExists = cn.Query<LibPoco.Users>("SELECT * FROM Users WHERE Email=@Email", new {Email = email});

                    if (userExists.Count() > 0)
                    {
                        throw new Exceptions.EmailAddressAlreadyUsedException("The email address requested is already used by another user.");
                    }
                }

                details.Email = email;
                details.FirstName = firstName;
                details.LastName = lastName;

                cn.Update(details);

            }
        }

        public LibPoco.Users GetProfile()
        {
            return details;
        }
    }
}

