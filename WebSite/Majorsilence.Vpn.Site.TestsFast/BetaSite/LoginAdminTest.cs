using System;
using System.Threading.Tasks;
using Dapper;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Majorsilence.Vpn.Site.TestsFast.BetaSite;

public class LoginAdminTest
{
    private readonly string betaKey = "abc1";
    private readonly string emailAddress = "testlogins@majorsilence.com";
    private readonly string password = "Password3";
    private int userid;

    [SetUp]
    public async Task Setup()
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
            , true, DatabaseSettings.Email);

        userid = await peterAccount.ExecuteAsync();
    }

    [TearDown]
    public void Cleanup()
    {
        using (var cn = DatabaseSettings.DbFactory)
        {
            cn.Open();
            cn.Execute("DELETE FROM Users WHERE Email = @email", new { email = emailAddress });
            cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new { Code = betaKey });
        }
    }


    [Test]
    public void CanLogin()
    {
        var mock = new Mock<ILogger>();
        var logger = mock.Object;
        var login = new Login(emailAddress, password, logger);
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

    [Test]
    public void InvalidUsernameLogin()
    {
        var mock = new Mock<ILogger>();
        var logger = mock.Object;
        var login = new Login("hithere", password, logger);
        login.Execute();

        Assert.That(login.LoggedIn, Is.False);
        Assert.That(login.IsAdmin, Is.False);
        Assert.That(login.Username, Is.EqualTo("hithere"));
        Assert.That(login.UserId, Is.EqualTo(-1));
    }

    [Test]
    public void InvalidPasswordLogin()
    {
        var mock = new Mock<ILogger>();
        var logger = mock.Object;
        var login = new Login(emailAddress, "wrong password", logger);
        login.Execute();

        Assert.That(login.LoggedIn, Is.False);
        Assert.That(login.IsAdmin, Is.False);
        Assert.That(login.Username, Is.EqualTo(emailAddress));
        Assert.That(login.UserId, Is.EqualTo(-1));
    }

    [Test]
    public void InvalidUsernameAndPasswordLogin()
    {
        var mock = new Mock<ILogger>();
        var logger = mock.Object;
        var login = new Login("hi there", "wrong password", logger);
        login.Execute();

        Assert.That(login.LoggedIn, Is.False);
        Assert.That(login.IsAdmin, Is.False);
        Assert.That(login.Username, Is.EqualTo("hi there"));
        Assert.That(login.UserId, Is.EqualTo(-1));
    }
}