using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Majorsilence.Vpn.Poco;

/// <summary>
/// Track the current payment rate for montly payments.  
/// When a payment is paid the rate is traced in the UserPayments table.
/// </summary>
[Dapper.Contrib.Extensions.Table("PaymentRates")]
public class PaymentRates
{
    public PaymentRates()
    {
    }

    public PaymentRates(decimal currentMonthlyRate, decimal currentYearlyRate)
    {
        CurrentMonthlyRate = currentMonthlyRate;
        CurrentYearlyRate = currentYearlyRate;
    }

    [Dapper.Contrib.Extensions.Key()] public int Id { get; set; }

    /// <summary>
    /// Amount for monthly payment at time of payment that will be charged to stripe subscription.
    /// </summary>
    public decimal CurrentMonthlyRate { get; set; }

    /// <summary>
    /// Amount for yearly payment at time of payment that will be charged to stripe subscription.
    /// </summary>
    public decimal CurrentYearlyRate { get; set; }
}