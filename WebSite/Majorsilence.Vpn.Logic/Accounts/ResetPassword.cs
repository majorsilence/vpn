using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Logic.Accounts;

public class ResetPassword
{
    private readonly IEmail email;
    private readonly GenerateResetCode generateCode;
    private readonly DatabaseSettings _dbSettings;

    private ResetPassword()
    {
    }

    public ResetPassword(IEmail email, IEncryptionKeysSettings keys,
        DatabaseSettings dbSettings)
    {
        this.email = email;
        generateCode = new GenerateResetCode(keys);
        _dbSettings = dbSettings;
    }

    public bool validateCode(string resetCode, string password)
    {
        var user = RetrieveUser("", resetCode);
        if (user != null)
            if (user.PasswordResetCode == resetCode)
            {
                var pwd = new CreatePasswords(password, user.FirstName + user.LastName);
                user.PasswordResetCode = "";
                user.Password = pwd.Password;
                user.Salt = pwd.Salt;
                using (var db = _dbSettings.DbFactory)
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

        return true;
    }

    public bool sendPasswordLink(string username)
    {
        var user = RetrieveUser(username, "");
        var ressetCode = generateCode.GeneratePasswordResetCode(username);
        user.PasswordResetCode = ressetCode;
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();
            using (var txn = db.BeginTransaction())
            {
                db.Update(user);

                txn.Commit();
            }
        }

        email.SendMail(string.Format(
                "Your Email Reset Code is: <a href=\"https://vpn.majorsilence.com/validatecode?resetcode={0}\">{1}</a>",
                HttpUtility.UrlEncode(ressetCode), ressetCode),
            "Password Reset", username, true, null, EmailTemplates.Generic);
        return true;
    }

    private Users RetrieveUser(string username, string code)
    {
        var user = new Users();
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();
            IEnumerable<Users> x = null;
            if (!string.IsNullOrEmpty(username))
                x = db.Query<Users>("SELECT * FROM Users WHERE Email=@Email",
                    new { Email = username });
            else if (!string.IsNullOrEmpty(code))
                x = db.Query<Users>("SELECT * FROM Users WHERE PasswordResetCode = @PasswordResetCode",
                    new { PasswordResetCode = code });

            if (x.Count() == 0)
                throw new InvalidDataException("Invalid Reset Code or User");
            if (x.Count() > 1)
                throw new InvalidDataException("Server Error Duplicate Codes");
            if (x.Count() == 1) user = x.First();
        }

        return user;
    }
}