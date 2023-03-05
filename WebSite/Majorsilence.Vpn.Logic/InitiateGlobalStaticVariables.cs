using System;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Logic;

public class InitiateGlobalStaticVariables
{
    private readonly DatabaseSettings _dbSettings;
    public InitiateGlobalStaticVariables(DatabaseSettings dbSettings)
    {
        _dbSettings = dbSettings;
    }
    
    public void Execute()
    {
        LoadCacheVariables();
    }
    
    private void LoadCacheVariables()
    {
        LoadSiteInfo();
    }
    
    private void LoadSiteInfo()
    {
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();


            var data = db.Query<SiteInfo>("SELECT * FROM SiteInfo").ToList();
            if (data.Count == 0)
                throw new InvalidDataException("Invalid data in SiteInfo.  No data found.");
            if (data.Count > 1)
                throw new InvalidDataException("Invalid data in SiteInfo.  Multiple rows found.");

            data.First().LiveSite = _dbSettings.IsLiveSite;
            db.Update(data.First());

            var paymenttypes = LoadLookupPaymentTypes();
            var rates = LoadCurrentRates();

            Helpers.SiteInfo.Initialize(data.First(), paymenttypes.Item1, rates.Item1, rates.Item2,
                paymenttypes.Item2);
        }
    }

    private Tuple<int, int> LoadLookupPaymentTypes()
    {
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();

            var data = db.Query<LookupPaymentType>("SELECT * FROM LookupPaymentType");
            if (data.Count() == 0)
                throw new InvalidDataException("Invalid data in LoadLookupPaymentTypes.  No data found.");

            var monthly = 1;
            var yearly = 2;

            foreach (var x in data)
                if (x.Code == "MONTHLY")
                    monthly = x.Id;
                else if (x.Code == "YEARLY") yearly = x.Id;

            return Tuple.Create(monthly, yearly);
        }
    }

    private Tuple<decimal, decimal> LoadCurrentRates()
    {
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();

            var data = db.Query<PaymentRates>("SELECT * FROM PaymentRates");
            if (data.Count() == 0 || data.Count() > 1)
                throw new InvalidDataException("Invalid data in PaymentRates.  To many or to few rows");

            return Tuple.Create(data.First().CurrentMonthlyRate, data.First().CurrentYearlyRate);
        }
    }
}