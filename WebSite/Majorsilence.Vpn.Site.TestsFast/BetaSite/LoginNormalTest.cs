using System;
using System.Linq;
using Dapper;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Poco;
using NUnit.Framework;

namespace Majorsilence.Vpn.Site.TestsFast.BetaSite;

public class LoginNormalTest
{
    private readonly string betaKey = "abc1";
    private readonly string emailAddress = "testloginsnotadmin@majorsilence.com";
    private readonly string password = "Password4";
    private int userid;

    [SetUp]
    public void Setup()
    {
        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = password,
                PasswordConfirm = password,
                BetaKey = betaKey
            }
            , false, InitializeSettings.Email);

        peterAccount.Execute();
        Console.WriteLine("Created account");
        using (var cn = InitializeSettings.DbFactory)
        {
            cn.Open();
            var users = cn.Query<Users>("SELECT * FROM Users WHERE Email = @Email", new { Email = emailAddress });
            Console.WriteLine("user count " + users.Count());
            if (users.Count() == 1)
                userid = users.First().Id;
            else
                throw new InvalidDataException("User for test not created");
        }
    }

    [TearDown]
    public void Cleanup()
    {
        using (var cn = InitializeSettings.DbFactory)
        {
            cn.Open();
            cn.Execute("DELETE FROM Users WHERE Email = @email", new { email = emailAddress });
            cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new { Code = betaKey });
        }
    }


    [Test]
    public void CanLogin()
    {
        var login = new Login(emailAddress, password);
        login.Execute();

        Console.WriteLine(login.LoggedIn);
        Assert.That(login.LoggedIn, Is.True);

        Console.WriteLine(login.IsAdmin);
        Assert.That(login.IsAdmin, Is.False);

        Console.WriteLine(login.Username);
        Assert.That(login.Username, Is.EqualTo(emailAddress));

        Console.WriteLine(login.UserId);
        Assert.That(login.UserId, Is.EqualTo(userid));
    }

    [Test]
    public void InvalidUsernameLogin()
    {
        var login = new Login("hithere", password);
        login.Execute();

        Assert.That(login.LoggedIn, Is.False);
        Assert.That(login.IsAdmin, Is.False);
        Assert.That(login.Username, Is.EqualTo("hithere"));
        Assert.That(login.UserId, Is.EqualTo(-1));
    }

    [Test]
    public void InvalidPasswordLogin()
    {
        var login = new Login(emailAddress, "wrong password");
        login.Execute();

        Assert.That(login.LoggedIn, Is.False);
        Assert.That(login.IsAdmin, Is.False);
        Assert.That(login.Username, Is.EqualTo(emailAddress));
        Assert.That(login.UserId, Is.EqualTo(-1));
    }

    [Test]
    public void InvalidUsernameAndPasswordLogin()
    {
        var login = new Login("hi there", "wrong password");
        login.Execute();

        Assert.That(login.LoggedIn, Is.False);
        Assert.That(login.IsAdmin, Is.False);
        Assert.That(login.Username, Is.EqualTo("hi there"));
        Assert.That(login.UserId, Is.EqualTo(-1));
    }
}