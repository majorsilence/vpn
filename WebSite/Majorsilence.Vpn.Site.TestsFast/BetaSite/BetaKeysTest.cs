using System.Linq;
using Dapper;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Email;
using NUnit.Framework;

namespace Majorsilence.Vpn.Site.TestsFast.BetaSite;

public class BetaKeysTest
{
    [Test]
    public void ValidDataTest()
    {
        var email = new FakeEmail();

        var test = new BetaKeys(email);
        var betakey = test.MailInvite("sometestemailbetakey@majorsilence.com");


        using (var cn = InitializeSettings.DbFactory)
        {
            cn.Open();
            var data = cn.Query<Poco.BetaKeys>("SELECT * FROM BetaKeys WHERE Code=@Code", new { Code = betakey });

            Assert.That(data.First().IsUsed, Is.False);
            Assert.That(data.First().IsSent, Is.True);
        }
    }
}