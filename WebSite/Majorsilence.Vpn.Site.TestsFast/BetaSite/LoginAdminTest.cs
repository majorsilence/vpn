using System;
using NUnit.Framework;
using Dapper;
using System.Linq;

namespace Majorsilence.Vpn.Site.TestsFast.BetaSite;

public class LoginAdminTest
{
    private readonly string emailAddress = "testlogins@majorsilence.com";
    private readonly string betaKey = "abc1";
    private readonly string password = "Password3";
    private int userid;

    [SetUp()]
    public void Setup()
    {
        var peterAccount = new Logic.Accounts.CreateAccount(
            new Logic.Accounts.CreateAccountInfo()
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = password,
                PasswordConfirm = password,
                BetaKey = betaKey
            }
            , true, Logic.InitializeSettings.Email);

        userid = peterAccount.Execute();
    }

    [TearDown()]
    public void Cleanup()
    {
        using (var cn = Logic.InitializeSettings.DbFactory)
        {
            cn.Open();
            cn.Execute("DELETE FROM Users WHERE Email = @email", new { email = emailAddress });
            cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new { Code = betaKey });
        }
    }


    [Test()]
    public void CanLogin()
    {
        var login = new Logic.Login(emailAddress, password);
        login.Execute();

        Console.WriteLine(login.LoggedIn);
        Assert.That(login.LoggedIn, Is.True);

        Console.WriteLine(login.IsAdmin);
        Assert.That(login.IsAdmin, Is.True);

        Console.WriteLine(login.Username);
        Assert.That(login.Username, Is.EqualTo(emailAddress));

        Console.WriteLine(login.UserId);
        Assert.That(login.UserId, Is.EqualTo(userid));
    }

    [Test()]
    public void InvalidUsernameLogin()
    {
        var login = new Logic.Login("hithere", password);
        login.Execute();

        Assert.That(login.LoggedIn, Is.False);
        Assert.That(login.IsAdmin, Is.False);
        Assert.That(login.Username, Is.EqualTo("hithere"));
        Assert.That(login.UserId, Is.EqualTo(-1));
    }

    [Test()]
    public void InvalidPasswordLogin()
    {
        var login = new Logic.Login(emailAddress, "wrong password");
        login.Execute();

        Assert.That(login.LoggedIn, Is.False);
        Assert.That(login.IsAdmin, Is.False);
        Assert.That(login.Username, Is.EqualTo(emailAddress));
        Assert.That(login.UserId, Is.EqualTo(-1));
    }

    [Test()]
    public void InvalidUsernameAndPasswordLogin()
    {
        var login = new Logic.Login("hi there", "wrong password");
        login.Execute();

        Assert.That(login.LoggedIn, Is.False);
        Assert.That(login.IsAdmin, Is.False);
        Assert.That(login.Username, Is.EqualTo("hi there"));
        Assert.That(login.UserId, Is.EqualTo(-1));
    }
}