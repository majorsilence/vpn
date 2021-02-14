using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Majorsilence.Vpn.Poco
{
    [Dapper.Contrib.Extensions.Table("UserPayments")]
    public class UserPayments
    {

        public UserPayments() { }
        public UserPayments(int userId, decimal amountPaid, DateTime createTime, int lookupPaymentTypeId)
        {
            this.UserId = userId;
            this.AmountPaid = amountPaid;
            this.CreateTime = createTime;
            this.LookupPaymentTypeId = lookupPaymentTypeId;
        }

        [Dapper.Contrib.Extensions.Key()]
        public int Id { get; set; }

        public int UserId { get; set; }

        public decimal AmountPaid { get; set; }

        public DateTime CreateTime { get; set; }

        /// <summary>
        /// The type of payment.  Monthly, Yearly, etc...
        /// Used to calculate when the account should expire if a new payment is not found.
        /// </summary>
        public int LookupPaymentTypeId { get; set; }

    }
}