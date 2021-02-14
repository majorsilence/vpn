using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;
using RestSharp;
using RestSharp.Authenticators;

namespace Majorsilence.Vpn.Logic.Payments
{
    public class Payment
    {
        private Payment()
        {
        }

        private int userId;
        private Majorsilence.Vpn.Poco.Users userInfo;

        public Payment(int userId)
        {
            this.userId = userId;

            using (var db = Setup.DbFactory)
            {
                db.Open();
                userInfo = db.Get<Majorsilence.Vpn.Poco.Users>(userId);
            }
        }

        public IEnumerable<Majorsilence.Vpn.Poco.UserPayments> History()
        {

            using (var db = Setup.DbFactory)
            {
                var x = db.Query<Majorsilence.Vpn.Poco.UserPayments>("SELECT * FROM UserPayments WHERE UserId=@UserId", 
                            new {UserId = userId});
                return x;
            }

        }

        /// <summary>
        /// Check if the current user account payment is expired.
        /// </summary>
        /// <returns></returns>
        public bool IsExpired()
        {
            if (userInfo == null)
            {
                return true;
            }
            else if (userInfo.Admin)
            {
                // Admins now get free accounts forever
                return false;
            }
            else if (ExpireDate() <= DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }

        public DateTime ExpireDate()
        {
           
            using (var db = Setup.DbFactory)
            {
                db.Open();
                var x = db.Query<Majorsilence.Vpn.Poco.UserPayments>("SELECT * FROM UserPayments WHERE UserId=@uid ORDER BY CreateTime DESC LIMIT 1", 
                            new { uid = userId });

                // var x = db.Select<Majorsilence.Vpn.Poco.UserPayments>("UserId={0}", userId);
                if (x.Count() == 0)
                {
                    return DateTime.UtcNow;
                }
      
                var payDate = x.First().CreateTime;
                var payType = x.First().LookupPaymentTypeId;

                DateTime expireDate = payDate;
                if (payType == Helpers.SiteInfo.MonthlyPaymentId)
                {
                    expireDate = expireDate.AddMonths(1);
                }
                else if (payType == Helpers.SiteInfo.YearlyPaymentId)
                {
                    expireDate = expireDate.AddYears(1);
                }
                else
                {
                    throw new Exceptions.InvalidDataException("Neither a monthly or yearly payment id has been found.");
                }


                // Add 1 day grace to allow payment to take place
                expireDate = expireDate.AddDays(1);
                return expireDate;
                    
 
            }
        }

        public void SaveUserPayment(decimal amount, DateTime createTime, int paymentCodeId)
        {
        
            if (userInfo == null)
            {
                throw new Majorsilence.Vpn.Logic.Exceptions.InvalidUserIdException(string.Format("The user attempting to make payments does not exist.", this.userId));
            }

            using (var db = Setup.DbFactory)
            {
                db.Open();

                db.Insert(new Majorsilence.Vpn.Poco.UserPayments(
                    userId, amount, createTime, paymentCodeId)
                );

            }
        }
    }
}