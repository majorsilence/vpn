using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace SiteTestsFast.BetaSite
{
    public class BetaKeysTest
    {
        public BetaKeysTest()
        {
        }

        [Test()]
        public void ValidDataTest()
        {
       
            var email = new LibLogic.Email.FakeEmail();

            var test = new LibLogic.Accounts.BetaKeys(email);
            string betakey = test.MailInvite("sometestemailbetakey@majorsilence.com");

   
            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();
                var data = cn.Query<LibPoco.BetaKeys>("SELECT * FROM BetaKeys WHERE Code=@Code", new {Code=betakey});

                Assert.That(data.First().IsUsed, Is.False);
                Assert.That(data.First().IsSent, Is.True);
            }
        }
    }
}

