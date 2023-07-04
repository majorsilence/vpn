using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Poco;
using NUnit.Framework;

namespace Majorsilence.Vpn.Site.TestsFast.LiveSite;

[TestFixture]
public class SiteInfo
{
    [SetUp]
    public void Setup()
    {
        using (var cn = LiveSite.Setup.DbSettings.DbFactory)
        {
            cn.Open();


            sinfo = cn.Query<Poco.SiteInfo>("SELECT * FROM SiteInfo").First();
            prates = cn.Query<PaymentRates>("SELECT * FROM PaymentRates").First();
        }
    }

    [TearDown]
    public void Teardown()
    {
        using (var cn = LiveSite.Setup.DbSettings.DbFactory)
        {
            cn.Open();

            cn.Update(sinfo);
            cn.Update(prates);
        }

        Logic.Helpers.SiteInfo.InitializeSimple(sinfo, prates.CurrentMonthlyRate, prates.CurrentYearlyRate);
        Logic.Helpers.SiteInfo.SaveCurrentSettingsToDb(LiveSite.Setup.DbSettings);
    }

    private Poco.SiteInfo sinfo;
    private PaymentRates prates;

    [Test]
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

        var monthlyRate = 6.34m;
        var yearlyRate = 62.47m;

        Logic.Helpers.SiteInfo.InitializeSimple(info, monthlyRate, yearlyRate);
        Logic.Helpers.SiteInfo.SaveCurrentSettingsToDb(LiveSite.Setup.DbSettings);


        using (var cn = LiveSite.Setup.DbSettings.DbFactory)
        {
            cn.Open();


            var changedInfo = cn.Query<Poco.SiteInfo>("SELECT * FROM SiteInfo").First();
            var changedRates = cn.Query<PaymentRates>("SELECT * FROM PaymentRates").First();
        }
    }
}