using System;
using System.Collections.Generic;

namespace VpnSite.Models
{
    public class Account
    {
        public Account()
        {

            var profileInfo = new LibLogic.Accounts.UserInfo(Helpers.SessionVariables.Instance.UserId).GetProfile();
            FirstName = profileInfo.FirstName;
            LastName = profileInfo.LastName;
            UsersEmail = profileInfo.Email;

            ChargeAmount = LibLogic.Helpers.SiteInfo.CurrentMonthlyRate.ToString("G29");
            ChargeAmountStripCents = LibLogic.Helpers.SiteInfo.CurrentMonthlyRateInCents;
            var payInfo = new LibLogic.Payments.Payment(Helpers.SessionVariables.Instance.UserId);
            AccountExpired = payInfo.IsExpired();
            PaymentHistory = payInfo.History();
        }

        public string ChargeAmount { get ; set; }

        public int ChargeAmountStripCents { get; set; }

        public string UsersEmail { get; set; }

        public bool AccountExpired { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IEnumerable<LibPoco.UserPayments> PaymentHistory { get; set; }

    }
}

