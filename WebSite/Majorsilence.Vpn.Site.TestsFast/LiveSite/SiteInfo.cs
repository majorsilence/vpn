using System;
using System.Linq;
using NUnit.Framework;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Site.TestsFast.LiveSite;

[TestFixture()]
public class SiteInfo
{
    public SiteInfo()
    {
    }

    private Majorsilence.Vpn.Poco.SiteInfo sinfo;
    private Poco.PaymentRates prates;

    [SetUp()]
    public void Setup()
    {
        using (var cn = Logic.InitializeSettings.DbFactory)
        {
            cn.Open();


            sinfo = cn.Query<Majorsilence.Vpn.Poco.SiteInfo>("SELECT * FROM SiteInfo").First();
            prates = cn.Query<Poco.PaymentRates>("SELECT * FROM PaymentRates").First();
        }
    }

    [TearDown()]
    public void Teardown()
    {
        using (var cn = Logic.InitializeSettings.DbFactory)
        {
            cn.Open();

            cn.Update<Majorsilence.Vpn.Poco.SiteInfo>(sinfo);
            cn.Update<Poco.PaymentRates>(prates);
        }

        Logic.Helpers.SiteInfo.InitializeSimple(sinfo, prates.CurrentMonthlyRate, prates.CurrentYearlyRate);
        Logic.Helpers.SiteInfo.SaveCurrentSettingsToDb();
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

        var monthlyRate = 6.34m;
        var yearlyRate = 62.47m;

        Logic.Helpers.SiteInfo.InitializeSimple(info, monthlyRate, yearlyRate);
        Logic.Helpers.SiteInfo.SaveCurrentSettingsToDb();


        using (var cn = Logic.InitializeSettings.DbFactory)
        {
            cn.Open();


            var changedInfo = cn.Query<Majorsilence.Vpn.Poco.SiteInfo>("SELECT * FROM SiteInfo").First();
            var changedRates = cn.Query<Poco.PaymentRates>("SELECT * FROM PaymentRates").First();
        }
    }
}