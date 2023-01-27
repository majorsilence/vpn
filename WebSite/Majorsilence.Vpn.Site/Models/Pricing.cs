using System;
using System.Globalization;

namespace Majorsilence.Vpn.Site.Models;

public class Pricing
{
    public Pricing()
    {
    }

    public string MonthlyPrice => string.Format(new CultureInfo("en-US"), "{0:C} {1}",
        Logic.Helpers.SiteInfo.CurrentMonthlyRate, Logic.Helpers.SiteInfo.Currency);

    public string YearlyPrice => string.Format(new CultureInfo("en-US"), "{0:C} {1}",
        Logic.Helpers.SiteInfo.CurrentYearlyRate, Logic.Helpers.SiteInfo.Currency);
}