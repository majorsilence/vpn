﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Poco;
using SiteInfo = Majorsilence.Vpn.Logic.Helpers.SiteInfo;

namespace Majorsilence.Vpn.Logic.Accounts;

public class CreateAccount
{
    private readonly DatabaseSettings _dbSettings;

    private readonly CreateAccountInfo details;
    private readonly IEmail email;

    private readonly bool isAdmin;
    private Poco.BetaKeys betaKey;

    private CreateAccount()
    {
    }

    public CreateAccount(CreateAccountInfo details, IEmail email,
        DatabaseSettings dbSettings)
    {
        this.details = details;
        this.email = email;
        _dbSettings = dbSettings;
    }

    public CreateAccount(CreateAccountInfo details, bool isAdmin, IEmail email)
    {
        this.details = details;
        this.isAdmin = isAdmin;
        this.email = email;
    }

    public async Task<int> ExecuteAsync()
    {
        ValidateData();

        await using (var db = _dbSettings.DbFactory)
        {
            await db.OpenAsync();


            await using (var txn = await db.BeginTransactionAsync())
            {
                var pwd = new CreatePasswords(details.Password, details.Firstname + details.Lastname);

                var userid = db.Insert(new Users(
                    details.Email,
                    pwd.Password,
                    pwd.Salt,
                    details.Firstname,
                    details.Lastname,
                    isAdmin,
                    DateTime.UtcNow,
                    "",
                    "",
                    "",
                    RequiresBetaKey()
                ));

                if (RequiresBetaKey())
                {
                    betaKey.IsUsed = true;
                    await db.UpdateAsync(betaKey);
                }

                await txn.CommitAsync();

                if (isAdmin == false) await EmailAccountCreation();

                return (int)userid;
            }
        }
    }

    private async Task EmailAccountCreation()
    {
        // TODO: the message should be read from a message template table
        var subject = $"{SiteInfo.SiteName} Account Created";
        var message = new StringBuilder();

        message.Append($"Welcome to {SiteInfo.SiteName}");
        message.Append("<br><br>");
        message.Append($"Your {SiteInfo.SiteName} account has been created.  ");
        message.Append($"You can login and start using your account anytime at {SiteInfo.SiteUrl}.");

        await email.SendMail(message.ToString(), subject, details.Email, true);
    }

    private void BetaKeySetup()
    {
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();
            var info = db.Query<Poco.BetaKeys>("SELECT * FROM BetaKeys WHERE Code = @code",
                new { code = details.BetaKey });
            if (info.Count() != 1)
                throw new InvalidBetaKeyException(
                    "There appears to have been an error validating the beta key.");

            betaKey = info.First();
            if (betaKey.IsUsed) throw new BetaKeyAlreadyUsedException("The beta key has already been used.");
        }
    }

    private bool RequiresBetaKey()
    {
        if (SiteInfo.LiveSite == false) return true;

        return false;
    }

    private void ValidateData()
    {
        if (details.Password != details.PasswordConfirm)
            throw new PasswordMismatchException("Password and confirm password do not match");

        if (details.Email != details.EmailConfirm)
            throw new EmailMismatchException("Email and confirm email do not match");

        if (details.Email.Trim() == "") throw new InvalidDataException("An email address must be filled in");

        if (details.Password.Trim() == "") throw new PasswordLengthException("A password must be filled in");

        if (details.Firstname.Trim() == "") throw new InvalidDataException("A first name must be filled in");

        if (details.Lastname.Trim() == "") throw new InvalidDataException("A last name must be filled in");

        if (RequiresBetaKey()) BetaKeySetup();

        ValidateEmailNotAlreadyUsed();
    }

    private void ValidateEmailNotAlreadyUsed()
    {
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();
            var info = db.Query<Poco.BetaKeys>("SELECT * FROM Users WHERE Email = @Email",
                new { details.Email });
            if (info.Count() > 0)
                throw new EmailAddressAlreadyUsedException(
                    "The email address specified for this account has already been used.");
        }
    }
}