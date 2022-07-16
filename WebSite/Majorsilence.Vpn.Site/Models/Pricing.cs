using System;
using System.Globalization;

namespace Majorsilence.Vpn.Site.Models
{
    public class Pricing
    {
        public Pricing()
        {
        }

        public string MonthlyPrice
        {
            get
            {
                return string.Format(new CultureInfo("en-US"), "{0:C} {1}", Majorsilence.Vpn.Logic.Helpers.SiteInfo.CurrentMonthlyRate, Majorsilence.Vpn.Logic.Helpers.SiteInfo.Currency);
            }
        }

        public string YearlyPrice
        {
            get
            {
                return string.Format(new CultureInfo("en-US"), "{0:C} {1}", Majorsilence.Vpn.Logic.Helpers.SiteInfo.CurrentYearlyRate, Majorsilence.Vpn.Logic.Helpers.SiteInfo.Currency);
            }
        }

    }
}

