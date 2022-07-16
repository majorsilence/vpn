using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Majorsilence.Vpn.Site.TestsFast.BetaSite
{
    public class BetaKeysTest
    {
        public BetaKeysTest()
        {
        }

        [Test()]
        public void ValidDataTest()
        {
       
            var email = new Majorsilence.Vpn.Logic.Email.FakeEmail();

            var test = new Majorsilence.Vpn.Logic.Accounts.BetaKeys(email);
            string betakey = test.MailInvite("sometestemailbetakey@majorsilence.com");

   
            using (var cn = Majorsilence.Vpn.Logic.InitializeSettings.DbFactory)
            {
                cn.Open();
                var data = cn.Query<Majorsilence.Vpn.Poco.BetaKeys>("SELECT * FROM BetaKeys WHERE Code=@Code", new {Code=betakey});

                Assert.That(data.First().IsUsed, Is.False);
                Assert.That(data.First().IsSent, Is.True);
            }
        }
    }
}

