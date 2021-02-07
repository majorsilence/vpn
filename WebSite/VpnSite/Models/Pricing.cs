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
                return string.Format(new CultureInfo("en-US"), "{0:C} {1}", LibLogic.Helpers.SiteInfo.CurrentMonthlyRate, LibLogic.Helpers.SiteInfo.Currency);
            }
        }

        public string YearlyPrice
        {
            get
            {
                return string.Format(new CultureInfo("en-US"), "{0:C} {1}", LibLogic.Helpers.SiteInfo.CurrentYearlyRate, LibLogic.Helpers.SiteInfo.Currency);
            }
        }

    }
}

