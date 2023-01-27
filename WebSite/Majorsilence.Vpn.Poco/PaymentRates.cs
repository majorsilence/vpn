using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Poco;

/// <summary>
///     Track the current payment rate for montly payments.
///     When a payment is paid the rate is traced in the UserPayments table.
/// </summary>
[Table("PaymentRates")]
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

    [Key] public int Id { get; set; }

    /// <summary>
    ///     Amount for monthly payment at time of payment that will be charged to stripe subscription.
    /// </summary>
    public decimal CurrentMonthlyRate { get; set; }

    /// <summary>
    ///     Amount for yearly payment at time of payment that will be charged to stripe subscription.
    /// </summary>
    public decimal CurrentYearlyRate { get; set; }
}