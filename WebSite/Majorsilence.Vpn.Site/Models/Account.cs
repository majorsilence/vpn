using System.Collections.Generic;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Payments;
using Majorsilence.Vpn.Poco;
using Microsoft.Extensions.Logging;
using SiteInfo = Majorsilence.Vpn.Logic.Helpers.SiteInfo;

namespace Majorsilence.Vpn.Site.Models;

public class Account : CustomViewLayout
{
    public Account(int userId, ILogger logger)
    {
        var profileInfo = new UserInfo(userId, logger).GetProfile();
        FirstName = profileInfo.FirstName;
        LastName = profileInfo.LastName;
        UsersEmail = profileInfo.Email;

        ChargeAmount = SiteInfo.CurrentMonthlyRate.ToString("G29");
        ChargeAmountStripCents = SiteInfo.CurrentMonthlyRateInCents;
        var payInfo = new Payment(userId);
        AccountExpired = payInfo.IsExpired();
        PaymentHistory = payInfo.History();
    }

    public string ChargeAmount { get; set; }

    public int ChargeAmountStripCents { get; set; }

    public string UsersEmail { get; set; }

    public bool AccountExpired { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public IEnumerable<UserPayments> PaymentHistory { get; set; }
}