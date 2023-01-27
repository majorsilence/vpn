using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Majorsilence.Vpn.Site.TestsFast.BetaSite;

public class BetaKeysTest
{
    public BetaKeysTest()
    {
    }

    [Test()]
    public void ValidDataTest()
    {
        var email = new Logic.Email.FakeEmail();

        var test = new Logic.Accounts.BetaKeys(email);
        var betakey = test.MailInvite("sometestemailbetakey@majorsilence.com");


        using (var cn = Logic.InitializeSettings.DbFactory)
        {
            cn.Open();
            var data = cn.Query<Poco.BetaKeys>("SELECT * FROM BetaKeys WHERE Code=@Code", new { Code = betakey });

            Assert.That(data.First().IsUsed, Is.False);
            Assert.That(data.First().IsSent, Is.True);
        }
    }
}