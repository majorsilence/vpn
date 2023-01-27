using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Logic.Helpers;

public static class SiteInfo
{
    private static Majorsilence.Vpn.Poco.SiteInfo info;


    public static void InitializeSimple(Majorsilence.Vpn.Poco.SiteInfo info, decimal currentMonthlyRate,
        decimal currentYearlyRate)
    {
        SiteInfo.info = info;
        CurrentMonthlyRate = currentMonthlyRate;
        CurrentMonthlyRateInCents = (int)currentMonthlyRate * 100;
        CurrentYearlyRate = currentYearlyRate;
    }

    public static void Initialize(Majorsilence.Vpn.Poco.SiteInfo info,
        int monthlyPaymentId,
        decimal currentMonthlyRate, decimal currentYearlyRate,
        int yearlyPaymentId)
    {
        SiteInfo.info = info;

        MonthlyPaymentId = monthlyPaymentId;
        CurrentMonthlyRate = currentMonthlyRate;
        CurrentMonthlyRateInCents = (int)currentMonthlyRate * 100;

        CurrentYearlyRate = currentYearlyRate;
        YearlyPaymentId = yearlyPaymentId;
    }

    public static void SaveCurrentSettingsToDb()
    {
        using (var cn = InitializeSettings.DbFactory)
        {
            cn.Open();
            using (var txn = cn.BeginTransaction())
            {
                cn.Update<Majorsilence.Vpn.Poco.SiteInfo>(info, txn);


                var dataPaymentRates = cn.Query<Poco.PaymentRates>("SELECT * FROM PaymentRates").ToList();
                if (dataPaymentRates.Count() == 0 || dataPaymentRates.Count() > 1)
                    throw new Exceptions.InvalidDataException("Invalid data in PaymentRates.  To many or to few rows");

                dataPaymentRates.First().CurrentMonthlyRate = CurrentMonthlyRate;
                dataPaymentRates.First().CurrentYearlyRate = CurrentYearlyRate;

                cn.Update<Poco.PaymentRates>(dataPaymentRates.First(), txn);

                txn.Commit();
            }
        }
    }

    public static int Id => info.Id;

    public static string VpnSshUser => info.VpnSshUser;

    public static int SshPort => info.SshPort;

    public static string AdminEmail => info.AdminEmail;

    public static string VpnSshPassword => info.VpnSshPassword;

    public static string SiteName => info.SiteName;

    public static string SiteUrl => info.SiteUrl;

    public static string StripeAPISecretKey => info.StripeAPISecretKey;

    public static string StripeAPIPublicKey => info.StripeAPIPublicKey;

    /// <summary>
    /// If site is not live you are required to enter a beta key to create an account
    /// </summary>
    public static bool LiveSite => info.LiveSite;

    public static string Currency => info.Currency;

    /// <summary>
    /// Id of the monthly payment in LookupsPaymentType
    /// </summary>
    public static int MonthlyPaymentId { get; private set; }


    public static decimal CurrentMonthlyRate { get; private set; }

    public static int CurrentMonthlyRateInCents { get; private set; }


    public static string StripePlanId => info.StripePlanId;

    public static decimal CurrentYearlyRate { get; private set; }

    /// <summary>
    /// Id of the yearly payment in LookupsPaymentType
    /// </summary>
    public static int YearlyPaymentId { get; private set; }
}