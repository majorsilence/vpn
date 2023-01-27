using System.Linq;
using Dapper;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Logic;

public class Login
{
    private Login()
    {
    }

    public Login(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public string Username { get; }

    public string Password { get; }

    public int UserId { get; private set; }

    public bool LoggedIn { get; private set; }

    public bool IsAdmin { get; private set; }


    private string RetrieveUserSalt()
    {
        var salt = "";
        using (var db = InitializeSettings.DbFactory)
        {
            var x = db.Query<Users>("SELECT * FROM Users WHERE Email=@Email",
                new { Email = Username });
            if (x.Count() != 1)
                throw new InvalidDataException(string.Format("User account ({0}) does not exist", Username));
            if (x.Count() == 1) salt = x.First().Salt;
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
        catch (InvalidDataException ex)
        {
            Logging.Log(ex);
            return;
        }

        // Create testing database

        var saltedpassword = Hashes.GetSHA512StringHash(Password, salt);

        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();
            var x = db.Query<Users>("SELECT * FROM Users WHERE Email=@Email AND Password = @Password",
                new { Email = Username, Password = saltedpassword });
            if (x.Count() == 0) return;

            if (x.Count() == 1)
            {
                LoggedIn = true;
                UserId = x.First().Id;
                IsAdmin = x.First().Admin;
                return;
            }

            throw new InvalidDataException(
                string.Format("Something is wrong.  Multple users were returned with this login: {0}.", Username));
        }
    }
}