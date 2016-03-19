using System;
using System.Linq;
using NUnit.Framework;
using Dapper;
using Dapper.Contrib.Extensions;

namespace SiteTestsFast.LiveSite
{
    [TestFixture()]
    public class SiteInfo
    {
        public SiteInfo()
        {
        }

        private LibPoco.SiteInfo sinfo;
        private LibPoco.PaymentRates prates;

        [SetUp()]
        public void Setup()
        {

            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();

               
                sinfo = cn.Query<LibPoco.SiteInfo>("SELECT * FROM SiteInfo").First();
                prates = cn.Query<LibPoco.PaymentRates>("SELECT * FROM PaymentRates").First();
               
            }

        }

        [TearDown()]
        public void Teardown()
        {

            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();

                cn.Update<LibPoco.SiteInfo>(sinfo);
                cn.Update<LibPoco.PaymentRates>(prates);

            }

            LibLogic.Helpers.SiteInfo.InitializeSimple(sinfo, prates.CurrentMonthlyRate, prates.CurrentYearlyRate);
            LibLogic.Helpers.SiteInfo.SaveCurrentSettingsToDb();

        }

        [Test()]
        public void EnsureSaveSavesTest()
        {
            var info = sinfo;
           
            info.AdminEmail = "asdfadsfdsf@majorsilence.com";
            info.LiveSite = true;
            info.SiteName = "A fancy site name";
            info.SiteUrl = "https://somesubdomain.majorsilence.com";
            info.SshPort = 46;
            info.StripeAPISecretKey = "stripeapisecretkey12345";
            info.StripeAPIPublicKey = "stripeapipublickey454545";
            info.VpnSshPassword = "vpnsshpassword454523";
            info.VpnSshUser = "vpnsshuserfadg3434";
            info.StripePlanId = "stripeplanidagafg";
            info.Currency = "currency%FE#";
          
            decimal monthlyRate = 6.34m;
            decimal yearlyRate = 62.47m;

            LibLogic.Helpers.SiteInfo.InitializeSimple(info, monthlyRate, yearlyRate);
            LibLogic.Helpers.SiteInfo.SaveCurrentSettingsToDb();


            using (var cn = LibLogic.Setup.DbFactory)
            {
                cn.Open();


                var changedInfo = cn.Query<LibPoco.SiteInfo>("SELECT * FROM SiteInfo").First();
                var changedRates = cn.Query<LibPoco.PaymentRates>("SELECT * FROM PaymentRates").First();

            }

        }

    }
}

