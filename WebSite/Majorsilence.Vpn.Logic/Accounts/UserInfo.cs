using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Logic.Accounts;

public class UserInfo
{
    private readonly Users details;

    private UserInfo()
    {
    }

    public UserInfo(int userid)
    {
        using (var cn = InitializeSettings.DbFactory)
        {
            cn.Open();

            details = cn.Get<Users>(userid);
        }
    }

    public static IEnumerable<Users> RetrieveUserList()
    {
        using (var cn = InitializeSettings.DbFactory)
        {
            cn.Open();

            return cn.Query<Users>("SELECT * FROM Users;");
        }
    }

    public void RemoveAccount()
    {
        using (var cn = InitializeSettings.DbFactory)
        {
            cn.Open();

            using (var txn = cn.BeginTransaction())
            {
                cn.Execute("DELETE FROM UserOpenVpnCerts WHERE UserId=@UserId", new { UserId = details.Id }, txn);
                cn.Execute("DELETE FROM UserPayments WHERE UserId=@UserId", new { UserId = details.Id }, txn);
                cn.Execute("DELETE FROM UserPptpInfo WHERE UserId=@UserId", new { UserId = details.Id }, txn);

                cn.Delete(details, txn);
                txn.Commit();
            }
        }
    }

    public void UpdatePassword(string oldPassword, string newPassword, string confirmNewPassword)
    {
        if (newPassword != confirmNewPassword)
            throw new InvalidDataException("New password and confirm new password do not match.");

        var login = new Login(details.Email, oldPassword);

        login.Execute();

        if (!login.LoggedIn) throw new InvalidDataException("Invalid old password");


        var pwd = new CreatePasswords(newPassword, details.FirstName + details.LastName);
        using (var cn = InitializeSettings.DbFactory)
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
            throw new InvalidDataException("An email address must be entered.");
        if (firstName.Trim() == "")
            throw new InvalidDataException("A first name must be entered.");
        if (lastName.Trim() == "") throw new InvalidDataException("A last name must be entered.");

        using (var cn = InitializeSettings.DbFactory)
        {
            cn.Open();

            if (email.Trim().ToLower() != details.Email.Trim().ToLower())
            {
                var userExists = cn.Query<Users>("SELECT * FROM Users WHERE Email=@Email", new { Email = email });

                if (userExists.Count() > 0)
                    throw new EmailAddressAlreadyUsedException(
                        "The email address requested is already used by another user.");
            }

            details.Email = email;
            details.FirstName = firstName;
            details.LastName = lastName;

            cn.Update(details);
        }
    }

    public Users GetProfile()
    {
        return details;
    }
}