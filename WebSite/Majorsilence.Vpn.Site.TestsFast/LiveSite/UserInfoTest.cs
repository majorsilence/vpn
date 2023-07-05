using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Poco;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Majorsilence.Vpn.Site.TestsFast.LiveSite;

public class UserInfoTest
{
    private readonly string emailAddress = "testuserinfo@majorsilence.com";
    private readonly string unicodeEmailAddress = "ಠ_ಠabc@majorsilence.com";


    [TearDown]
    public void Cleanup()
    {
        using (var cn = Setup.DbSettings.DbFactory)
        {
            cn.Open();
            cn.Execute("DELETE FROM Users WHERE Email = @email", new { email = emailAddress });
            cn.Execute("DELETE FROM Users WHERE Email = @email", new { email = unicodeEmailAddress });
        }
    }

    private bool AccountExists(string email)
    {
        using (var cn = Setup.DbSettings.DbFactory)
        {
            cn.Open();
            var users = cn.Query<Users>("SELECT * FROM Users WHERE Email = @Email", new { Email = email });
            return users.Count() == 1;
        }
    }

    [Test]
    public async Task RetreiveUserInfoTest()
    {
        Assert.That(AccountExists(emailAddress), Is.False);

        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = ""
            }
            , false, new FakeEmail());

        var userid = await peterAccount.ExecuteAsync();

        Assert.That(AccountExists(emailAddress), Is.True);

        var mock = new Mock<ILogger>();
        var logger = mock.Object;
        var info = new UserInfo(userid, logger, Setup.DbSettings);
        var profile = info.GetProfile();

        Assert.That("Peter", Is.EqualTo(profile.FirstName));
        Assert.That("Gill", Is.EqualTo(profile.LastName));
        Assert.That(emailAddress, Is.EqualTo(profile.Email));
        Assert.That(false, Is.EqualTo(profile.Admin));
        Assert.That(false, Is.EqualTo(profile.IsBetaUser));
    }

    [Test]
    public async Task UpdateUserInfoTest()
    {
        Assert.That(AccountExists(emailAddress), Is.False);

        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = ""
            }
            , false, new FakeEmail());

        var userid = await peterAccount.ExecuteAsync();

        Assert.That(AccountExists(emailAddress), Is.True);

        var mock = new Mock<ILogger>();
        var logger = mock.Object;
        var info = new UserInfo(userid, logger, Setup.DbSettings);
        var profile = info.GetProfile();

        Assert.That("Peter", Is.EqualTo(profile.FirstName));
        Assert.That("Gill", Is.EqualTo(profile.LastName));
        Assert.That(emailAddress, Is.EqualTo(profile.Email));
        Assert.That(false, Is.EqualTo(profile.Admin));
        Assert.That(false, Is.EqualTo(profile.IsBetaUser));


        info.UpdateProfile(unicodeEmailAddress, "Happy", "Dude");
        Assert.That("Happy", Is.EqualTo(profile.FirstName));
        Assert.That("Dude", Is.EqualTo(profile.LastName));
        Assert.That(unicodeEmailAddress, Is.EqualTo(profile.Email));


        var info2 = new UserInfo(userid, logger, Setup.DbSettings);
        var profile2 = info2.GetProfile();
        Assert.That("Happy", Is.EqualTo(profile2.FirstName));
        Assert.That("Dude", Is.EqualTo(profile2.LastName));
        Assert.That(unicodeEmailAddress, Is.EqualTo(profile2.Email));
    }


    [Test]
    public async Task UpdateUserInfoInvalidFirstNameTest()
    {
        Assert.That(AccountExists(emailAddress), Is.False);

        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = ""
            }
            , false, new FakeEmail());

        var userid = await peterAccount.ExecuteAsync();

        Assert.That(AccountExists(emailAddress), Is.True);

        var mock = new Mock<ILogger>();
        var logger = mock.Object;
        var info = new UserInfo(userid, logger, Setup.DbSettings);
        // var profile = info.GetProfile();

        Assert.Throws<InvalidDataException>(() => info.UpdateProfile(unicodeEmailAddress, "", "Dude"));
    }

    [Test]
    public async Task UpdateUserInfoInvalidLastNameTest()
    {
        Assert.That(AccountExists(emailAddress), Is.False);

        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = ""
            }
            , false, new FakeEmail());

        var userid = await peterAccount.ExecuteAsync();

        Assert.That(AccountExists(emailAddress), Is.True);

        var mock = new Mock<ILogger>();
        var logger = mock.Object;
        var info = new UserInfo(userid, logger, Setup.DbSettings);
        var profile = info.GetProfile();

        Assert.Throws<InvalidDataException>(() =>
            info.UpdateProfile(unicodeEmailAddress, "Happy", ""));
    }

    [Test]
    public async Task UpdateUserInfoInvalidEmailTest()
    {
        Assert.That(AccountExists(emailAddress), Is.False);

        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = ""
            }
            , false, new FakeEmail());

        var userid = await peterAccount.ExecuteAsync();

        Assert.That(AccountExists(emailAddress), Is.True);

        var mock = new Mock<ILogger>();
        var logger = mock.Object;
        var info = new UserInfo(userid, logger, Setup.DbSettings);
        var profile = info.GetProfile();

        Assert.Throws<InvalidDataException>(() => info.UpdateProfile("", "Happy", "Dude"));
    }

    [Test]
    public async Task UpdateUserInfoEmailAddressAlreadyUsedTest()
    {
        Assert.That(AccountExists(emailAddress), Is.False);

        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = ""
            }
            , false, new FakeEmail());

        var userid = await peterAccount.ExecuteAsync();

        var peterAccount2 = new CreateAccount(
            new CreateAccountInfo
            {
                Email = unicodeEmailAddress,
                EmailConfirm = unicodeEmailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password2",
                PasswordConfirm = "Password2",
                BetaKey = ""
            }
            , false, new FakeEmail());

        await peterAccount2.ExecuteAsync();


        Assert.That(AccountExists(emailAddress), Is.True);

        var mock = new Mock<ILogger>();
        var logger = mock.Object;
        var info = new UserInfo(userid, logger, Setup.DbSettings);
        var profile = info.GetProfile();

        Assert.Throws<EmailAddressAlreadyUsedException>(() =>
            info.UpdateProfile(unicodeEmailAddress, "Happy", "Dude"));
    }
}