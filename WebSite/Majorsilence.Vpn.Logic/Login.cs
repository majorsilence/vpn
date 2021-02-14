using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Logic
{
    public class Login
    {

        private Login()
        {
        }

        public string Username { get; private set; }

        public string Password{ get; private set; }

        public int UserId{ get; private set; }

        public bool LoggedIn { get; private set; }

        public bool IsAdmin { get; private set; }

        public Login(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }


        private string RetrieveUserSalt()
        {
            string salt = "";
            using (IDbConnection db = Setup.DbFactory)
            {
                var x = db.Query<Majorsilence.Vpn.Poco.Users>("SELECT * FROM Users WHERE Email=@Email",
                            new {Email = Username});
                if (x.Count() != 1)
                {
                    throw new Exceptions.InvalidDataException(string.Format("User account ({0}) does not exist", Username));
                }
                else if (x.Count() == 1)
                {
                    salt = x.First().Salt;
                }

            }
            return salt;
        }

        public void Execute()
        {
            LoggedIn = false;
            IsAdmin = false;
            UserId = -1;

            string salt;
            try
            {
                salt = RetrieveUserSalt();
            }
            catch (Exceptions.InvalidDataException ex)
            {
                Majorsilence.Vpn.Logic.Helpers.Logging.Log(ex);
                return;
            }

            // Create testing database
           
            var saltedpassword = Helpers.Hashes.GetSHA512StringHash(Password, salt);

            using (var db = Setup.DbFactory)
            {
                db.Open();
                var x = db.Query<Majorsilence.Vpn.Poco.Users>("SELECT * FROM Users WHERE Email=@Email AND Password = @Password", 
                            new {Email = Username, Password = saltedpassword});
                if (x.Count() == 0)
                {
                    return;
                }
                else if (x.Count() == 1)
                {
                    LoggedIn = true;
                    UserId = x.First().Id;
                    IsAdmin = x.First().Admin;
                    return;
                }
                else
                {
                    throw new Exceptions.InvalidDataException(
                        string.Format("Something is wrong.  Multple users were returned with this login: {0}.", Username));
                }
            }
        }



    }
}