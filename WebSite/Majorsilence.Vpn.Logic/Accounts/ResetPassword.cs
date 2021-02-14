using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;

namespace LibLogic.Accounts
{
    public class ResetPassword
    {
        private ResetPassword()
        {
        }

        public ResetPassword(Email.IEmail email)
        { 
            this.email = email;
        }

        private Email.IEmail email;
        private Helpers.GenerateResetCode generateCode = new Helpers.GenerateResetCode();

        public bool validateCode(string resetCode, string password)
        {
            LibPoco.Users user = RetrieveUser("", resetCode);
            if (user != null)
            {
                if (user.PasswordResetCode == resetCode)
                {
                    var pwd = new CreatePasswords(password, user.FirstName + user.LastName);
                    user.PasswordResetCode = "";
                    user.Password = pwd.Password;
                    user.Salt = pwd.Salt;
                    using (var db = Setup.DbFactory)
                    {
                        db.Open();

                        using (var txn = db.BeginTransaction())
                        {
                            db.Update(user);

                            txn.Commit();
                        }

                    }

                    email.SendMail("Your Password was Reset", "Password Reset", user.Email, false);
                    return true;
                }
            }
            
            return true;
        }

        public bool sendPasswordLink(string username)
        {

            LibPoco.Users user = RetrieveUser(username, "");
            string ressetCode = generateCode.GeneratePasswordResetCode(username);
            user.PasswordResetCode = ressetCode;
            using (var db = Setup.DbFactory)
            {
                db.Open();
                using (var txn = db.BeginTransaction())
                {

                    db.Update(user);

                    txn.Commit();
                }

            }
                
            email.SendMail(string.Format("Your Email Reset Code is: <a href=\"https://majorsilencevpn.com/validatecode?resetcode={0}\">{1}</a>", 
                System.Web.HttpUtility.UrlEncode(ressetCode), ressetCode), 
                "Password Reset", username, true, null, LibLogic.Email.EmailTemplates.Generic);
            return true;
        }

        private LibPoco.Users RetrieveUser(string username, string code)
        {
            LibPoco.Users user = new LibPoco.Users();
            using (var db = Setup.DbFactory)
            {
                db.Open();
                IEnumerable<LibPoco.Users> x = null;
                if (!string.IsNullOrEmpty(username))
                {
                    x = db.Query<LibPoco.Users>("SELECT * FROM Users WHERE Email=@Email", 
                        new {Email = username});
                }
                else if (!string.IsNullOrEmpty(code))
                {
                    x = db.Query<LibPoco.Users>("SELECT * FROM Users WHERE PasswordResetCode = @PasswordResetCode",
                        new {PasswordResetCode = code});
                }
	
                if (x.Count() == 0)
                {
                    throw new Exceptions.InvalidDataException("Invalid Reset Code or User");
                }
                else if (x.Count() > 1)
                {
                    throw new Exceptions.InvalidDataException("Server Error Duplicate Codes");
                }
                else if (x.Count() == 1)
                {
                    user = x.First();
                }

            }
            return user;
        }
    }
}
