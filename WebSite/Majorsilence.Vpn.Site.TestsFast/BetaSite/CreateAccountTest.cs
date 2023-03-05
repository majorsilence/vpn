using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Poco;
using NUnit.Framework;

namespace Majorsilence.Vpn.Site.TestsFast.BetaSite;

public class CreateAccountTest
{
    private readonly string betaKey = "abc1";
    private readonly string betaKey2 = "abc2";
    private readonly string emailAddress = "test@majorsilence.com";
    private readonly string unicodeEmailAddress = "ಠ_ಠ@majorsilence.com";

    [TearDown]
    public void Cleanup()
    {
        using (var cn = DatabaseSettings.DbFactory)
        {
            cn.Open();
            cn.Execute("DELETE FROM Users WHERE Email = @email", new { email = emailAddress });
            cn.Execute("DELETE FROM Users WHERE Email = @email", new { email = unicodeEmailAddress });
            cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new { Code = betaKey });
            cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new { Code = betaKey2 });
        }
    }

    private bool AccountExists(string email)
    {
        using (var cn = DatabaseSettings.DbFactory)
        {
            cn.Open();
            var users = cn.Query<Users>("SELECT * FROM Users WHERE Email = @Email", new { Email = email });
            return users.Count() == 1;
        }
    }

    [Test]
    public async Task ValidDataTest()
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
                BetaKey = betaKey
            }
            , true, DatabaseSettings.Email);

        await peterAccount.ExecuteAsync();

        Assert.That(AccountExists(emailAddress), Is.True);
    }

    [Test]
    public async Task DuplicateEmailTest()
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
                BetaKey = betaKey
            }
            , false, DatabaseSettings.Email);

        await peterAccount.ExecuteAsync();


        var peterAccount2 = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey2
            }
            , false, DatabaseSettings.Email);

        Assert.Throws<EmailAddressAlreadyUsedException>(async () => await peterAccount2.ExecuteAsync());

        Assert.That(AccountExists(emailAddress), Is.True);
    }

    [Test]
    public void PasswordMismatch()
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
                PasswordConfirm = "A different password",
                BetaKey = betaKey
            }
            , true, DatabaseSettings.Email);

        Assert.Throws<PasswordMismatchException>(async () => await peterAccount.ExecuteAsync());
    }

    [Test]
    public void PasswordLengthTest()
    {
        Assert.That(AccountExists(emailAddress), Is.False);

        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "",
                PasswordConfirm = "",
                BetaKey = betaKey
            }
            , true, DatabaseSettings.Email);


        Assert.Throws<PasswordLengthException>(async () => await peterAccount.ExecuteAsync());
    }

    [Test]
    public void EmailMismatch()
    {
        Assert.That(AccountExists(emailAddress), Is.False);

        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = "AveryDefadfasdemail@majorsilence.com",
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey
            }
            , true, DatabaseSettings.Email);

        Assert.Throws<EmailMismatchException>(async () => await peterAccount.ExecuteAsync());
    }

    [Test]
    public async Task TestUnicode()
    {
        Assert.That(AccountExists(emailAddress), Is.False);

        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = unicodeEmailAddress,
                EmailConfirm = unicodeEmailAddress,
                Firstname = "§",
                Lastname = "¼",
                Password = "β",
                PasswordConfirm = "β",
                BetaKey = betaKey
            }
            , true, DatabaseSettings.Email);

        await peterAccount.ExecuteAsync();

        using (var cn = DatabaseSettings.DbFactory)
        {
            cn.Open();
            var data = cn.Query<Users>("SELECT * FROM Users WHERE Email = @email",
                new { email = unicodeEmailAddress });

            Assert.That(data.First().IsBetaUser, Is.True);
            Assert.That(data.First().Admin, Is.True);
            Assert.That(data.First().FirstName, Is.EqualTo("§"));
            Assert.That(data.First().LastName, Is.EqualTo("¼"));
        }
    }

    [Test]
    public async Task IsAdmin()
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
                BetaKey = betaKey
            }
            , true, DatabaseSettings.Email);

        await peterAccount.ExecuteAsync();

        using (var cn = DatabaseSettings.DbFactory)
        {
            cn.Open();
            var data = cn.Query<Users>("SELECT * FROM Users WHERE Email = @email", new { email = emailAddress });

            Assert.That(data.First().Admin, Is.True);
        }
    }

    [Test]
    public async Task IsNotAdmin()
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
                BetaKey = betaKey
            }
            , false, DatabaseSettings.Email);

        await peterAccount.ExecuteAsync();

        using (var cn = DatabaseSettings.DbFactory)
        {
            cn.Open();
            var data = cn.Query<Users>("SELECT * FROM Users WHERE Email = @email", new { email = emailAddress });

            Assert.That(data.First().Admin, Is.False);
        }
    }

    [Test]
    public void FirstNameMissing()
    {
        Assert.That(AccountExists(emailAddress), Is.False);

        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey
            }
            , true, DatabaseSettings.Email);

        Assert.Throws<InvalidDataException>(async () => await peterAccount.ExecuteAsync());
    }

    [Test]
    public void LastNameMissing()
    {
        Assert.That(AccountExists(emailAddress), Is.False);

        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey
            }
            , true, DatabaseSettings.Email);

        Assert.Throws<InvalidDataException>(async() => await peterAccount.ExecuteAsync());
    }

    [Test]
    public void EmailAddressMissing()
    {
        Assert.That(AccountExists(emailAddress), Is.False);

        var peterAccount = new CreateAccount(
            new CreateAccountInfo
            {
                Email = "",
                EmailConfirm = "",
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey
            }
            , true, DatabaseSettings.Email);

        Assert.Throws<InvalidDataException>(async () => await peterAccount.ExecuteAsync());
    }

    [Test]
    public async Task BetaKeyAlreadyInUse()
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
                BetaKey = betaKey
            }
            , true, DatabaseSettings.Email);

        await peterAccount.ExecuteAsync();

        var peterAccount2 = new CreateAccount(
            new CreateAccountInfo
            {
                Email = emailAddress,
                EmailConfirm = emailAddress,
                Firstname = "Peter",
                Lastname = "Gill",
                Password = "Password1",
                PasswordConfirm = "Password1",
                BetaKey = betaKey
            }
            , true, DatabaseSettings.Email);

        Assert.Throws<BetaKeyAlreadyUsedException>(async() => await peterAccount2.ExecuteAsync());


        Assert.That(AccountExists(emailAddress), Is.True);
    }


    [Test]
    public void InvalidBetaKey()
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
                BetaKey = "A Fake Beta Key"
            }
            , true, DatabaseSettings.Email);


        Assert.Throws<InvalidBetaKeyException>(async () => await peterAccount.ExecuteAsync());


        Assert.That(AccountExists(emailAddress), Is.False);
    }

    [Test]
    public void EmptyBetaKey()
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
            , true, DatabaseSettings.Email);

        Assert.Throws<InvalidBetaKeyException>(async() => await peterAccount.ExecuteAsync());


        Assert.That(AccountExists(emailAddress), Is.False);
    }
}