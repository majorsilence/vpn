using System;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Poco;

[Table("UserPayments")]
public class UserPayments
{
    public UserPayments()
    {
    }

    public UserPayments(int userId, decimal amountPaid, DateTime createTime, int lookupPaymentTypeId)
    {
        UserId = userId;
        AmountPaid = amountPaid;
        CreateTime = createTime;
        LookupPaymentTypeId = lookupPaymentTypeId;
    }

    [Key] public int Id { get; set; }

    public int UserId { get; set; }

    public decimal AmountPaid { get; set; }

    public DateTime CreateTime { get; set; }

    /// <summary>
    ///     The type of payment.  Monthly, Yearly, etc...
    ///     Used to calculate when the account should expire if a new payment is not found.
    /// </summary>
    public int LookupPaymentTypeId { get; set; }
}