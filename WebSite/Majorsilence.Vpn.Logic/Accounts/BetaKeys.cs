using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Poco;
using SiteInfo = Majorsilence.Vpn.Logic.Helpers.SiteInfo;

namespace Majorsilence.Vpn.Logic.Accounts;

public class BetaKeys
{
    private readonly DatabaseSettings _dbSettings;
    private readonly IEmail email;

    public BetaKeys(IEmail email, DatabaseSettings dbSettings)
    {
        this.email = email;
        _dbSettings = dbSettings;
    }

    public int UnsuedKeyCount()
    {
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();

            var data = db.Query<int>("SELECT count(id) FROM BetaKeys WHERE IsUsed=0");
            return data.First();
        }
    }

    private string RetrieveAndMarkSendKey()
    {
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();
            using (var txn = db.BeginTransaction())
            {
                var data = db.Query<Poco.BetaKeys>(
                    "SELECT * FROM BetaKeys WHERE IsUsed=0 AND IsSent=0 LIMIT 1", null, txn);
                data.First().IsSent = true;
                db.Update(data.First(), txn);

                txn.Commit();

                return data.First().Code;
            }
        }
    }

    /// <summary>
    ///     Mails the invite.
    /// </summary>
    /// <returns>The betakey code</returns>
    /// <param name="emailAddress">Email address.</param>
    public async Task<string> MailInvite(string emailAddress)
    {
        var betakey = RetrieveAndMarkSendKey();
        var subject = SiteInfo.SiteName + " Invite";

        var signupLink = string.Format("{0}/?betaemail={1}&betacode={2}",
            SiteInfo.SiteUrl,
            HttpUtility.HtmlEncode(emailAddress),
            HttpUtility.HtmlEncode(betakey));

        // TODO: the message should be read from a message template table
        var message = $"You have been invited to signup at <a href=\"{signupLink}\">{SiteInfo.SiteName}</a>.";
        message += " Your beta key is below. <br><br>";
        message += $"<strong>{betakey}</strong>";

        await email.SendMail(message, subject, emailAddress, true, null, EmailTemplates.BetaKey);

        return betakey;
    }

    private string GenerateKeyAndMarkSent()
    {
        var betaKey = Guid.NewGuid().ToString();
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();

            var data = new Poco.BetaKeys(betaKey, false, true);
            db.Insert(data);
        }

        return betaKey;
    }

    public async Task<string> MailInvite(string emailAddressSendTo, int sentFromUserId)
    {
        string sentFromFirstName;
        string sentFromLastName;
        string sentFromEmailAddress;
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();

            var data = db.Get<Users>(sentFromUserId);

            sentFromFirstName = data.FirstName;
            sentFromLastName = data.LastName;
            sentFromEmailAddress = data.Email;
        }


        var betakey = GenerateKeyAndMarkSent();
        var subject = SiteInfo.SiteName + " Invite";

        var signupLink =
            $"{SiteInfo.SiteUrl}/?betaemail={HttpUtility.HtmlEncode(emailAddressSendTo)}&betacode={HttpUtility.HtmlEncode(betakey)}";

        var message = string.Format(
            "You have been invited to signup at <a href=\"{0}\">{1}</a> by {2} {3} ({4}).<br><br>",
            signupLink, SiteInfo.SiteName, sentFromFirstName, sentFromLastName,
            HttpUtility.HtmlEncode(sentFromEmailAddress));

        message += " Your beta key is below. <br><br>";
        message += $"<strong>{betakey}</strong>";

        await email.SendMail(message, subject, emailAddressSendTo, true, null, EmailTemplates.BetaKey);

        return betakey;
    }
}