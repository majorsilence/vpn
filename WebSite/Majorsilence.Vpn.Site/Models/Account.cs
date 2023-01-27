using System;
using System.Collections.Generic;

namespace Majorsilence.Vpn.Site.Models;

public class Account : CustomViewLayout
{
    public Account(int userId)
    {
        var profileInfo = new Logic.Accounts.UserInfo(userId).GetProfile();
        FirstName = profileInfo.FirstName;
        LastName = profileInfo.LastName;
        UsersEmail = profileInfo.Email;

        ChargeAmount = Logic.Helpers.SiteInfo.CurrentMonthlyRate.ToString("G29");
        ChargeAmountStripCents = Logic.Helpers.SiteInfo.CurrentMonthlyRateInCents;
        var payInfo = new Logic.Payments.Payment(userId);
        AccountExpired = payInfo.IsExpired();
        PaymentHistory = payInfo.History();
    }

    public string ChargeAmount { get; set; }

    public int ChargeAmountStripCents { get; set; }

    public string UsersEmail { get; set; }

    public bool AccountExpired { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public IEnumerable<Poco.UserPayments> PaymentHistory { get; set; }
}