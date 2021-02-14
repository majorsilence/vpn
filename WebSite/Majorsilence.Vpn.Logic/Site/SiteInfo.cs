using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Logic.Helpers
{
    public static class SiteInfo
    {
        private static Majorsilence.Vpn.Poco.SiteInfo info;


        public static void InitializeSimple(Majorsilence.Vpn.Poco.SiteInfo info, decimal currentMonthlyRate, decimal currentYearlyRate)
        {
            SiteInfo.info = info;
            SiteInfo.CurrentMonthlyRate = currentMonthlyRate;
            SiteInfo.CurrentMonthlyRateInCents = (int)currentMonthlyRate * 100;
            SiteInfo.CurrentYearlyRate = currentYearlyRate;
        }

        public static void Initialize(Majorsilence.Vpn.Poco.SiteInfo info,
                                      int monthlyPaymentId,
                                      decimal currentMonthlyRate, decimal currentYearlyRate,
                                      int yearlyPaymentId)
        {
            SiteInfo.info = info;

            SiteInfo.MonthlyPaymentId = monthlyPaymentId;
            SiteInfo.CurrentMonthlyRate = currentMonthlyRate;
            SiteInfo.CurrentMonthlyRateInCents = (int)currentMonthlyRate * 100;

            SiteInfo.CurrentYearlyRate = currentYearlyRate;
            SiteInfo.YearlyPaymentId = yearlyPaymentId;

        }

        public static void SaveCurrentSettingsToDb()
        {

            using (var cn = Setup.DbFactory)
            {
                cn.Open();
                using (var txn = cn.BeginTransaction())
                {
               
                    cn.Update<Majorsilence.Vpn.Poco.SiteInfo>(info, txn);


                    var dataPaymentRates = cn.Query<Majorsilence.Vpn.Poco.PaymentRates>("SELECT * FROM PaymentRates").ToList();
                    if (dataPaymentRates.Count() == 0 || dataPaymentRates.Count() > 1)
                    {
                        throw new Majorsilence.Vpn.Logic.Exceptions.InvalidDataException("Invalid data in PaymentRates.  To many or to few rows");
                    }

                    dataPaymentRates.First().CurrentMonthlyRate = SiteInfo.CurrentMonthlyRate;
                    dataPaymentRates.First().CurrentYearlyRate = SiteInfo.CurrentYearlyRate;

                    cn.Update<Majorsilence.Vpn.Poco.PaymentRates>(dataPaymentRates.First(), txn);

                    txn.Commit();
                }
            }
                


        }

        public static int Id
        {
            get { return info.Id; }
        }

        public static string VpnSshUser
        {
            get { return info.VpnSshUser; }
        }

        public static int SshPort
        {
            get { return info.SshPort; }
        }

        public static string AdminEmail
        {
            get{ return info.AdminEmail; }
        }

        public static string VpnSshPassword
        {
            get { return info.VpnSshPassword; }
        }

        public static string SiteName
        {
            get{ return info.SiteName; }
        }

        public static string SiteUrl
        {
            get{ return info.SiteUrl; }
        }

        public static string StripeAPISecretKey
        {
            get{ return info.StripeAPISecretKey; }
        }

        public static string StripeAPIPublicKey
        {
            get{ return info.StripeAPIPublicKey; }
        }

        /// <summary>
        /// If site is not live you are required to enter a beta key to create an account
        /// </summary>
        public static bool LiveSite
        {
            get { return info.LiveSite; }
        }

        public static string Currency
        { 
            get
            {
                return info.Currency;
            }
        
        }

        /// <summary>
        /// Id of the monthly payment in LookupsPaymentType
        /// </summary>
        public static int MonthlyPaymentId { get; private set; }


        public static decimal CurrentMonthlyRate { get; private set; }

        public static int CurrentMonthlyRateInCents { get; private set; }


        public static string StripePlanId
        { 
            get
            { 
                return info.StripePlanId;
            } 
        }

        public static decimal CurrentYearlyRate { get; private set; }

        /// <summary>
        /// Id of the yearly payment in LookupsPaymentType
        /// </summary>
        public static int YearlyPaymentId { get; private set; }



    }
}
