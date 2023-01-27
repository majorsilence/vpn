using System.Globalization;
using Majorsilence.Vpn.Logic.Helpers;

namespace Majorsilence.Vpn.Site.Models;

public class Pricing
{
    public string MonthlyPrice => string.Format(new CultureInfo("en-US"), "{0:C} {1}",
        SiteInfo.CurrentMonthlyRate, SiteInfo.Currency);

    public string YearlyPrice => string.Format(new CultureInfo("en-US"), "{0:C} {1}",
        SiteInfo.CurrentYearlyRate, SiteInfo.Currency);
}