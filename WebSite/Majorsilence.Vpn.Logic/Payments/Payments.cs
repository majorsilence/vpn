using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Poco;
using SiteInfo = Majorsilence.Vpn.Logic.Helpers.SiteInfo;

namespace Majorsilence.Vpn.Logic.Payments;

public class Payment
{
    private readonly DatabaseSettings _dbSettings;
    private readonly int userId;
    private readonly Users userInfo;

    private Payment()
    {
    }

    public Payment(int userId, DatabaseSettings dbSettings)
    {
        this.userId = userId;
        _dbSettings = dbSettings;

        using (var db = _dbSettings.DbFactory)
        {
            db.Open();
            userInfo = db.Get<Users>(userId);
        }
    }

    public IEnumerable<UserPayments> History()
    {
        using (var db = _dbSettings.DbFactory)
        {
            var x = db.Query<UserPayments>("SELECT * FROM UserPayments WHERE UserId=@UserId",
                new { UserId = userId });
            return x;
        }
    }

    /// <summary>
    ///     Check if the current user account payment is expired.
    /// </summary>
    /// <returns></returns>
    public bool IsExpired()
    {
        if (userInfo == null)
            return true;
        if (userInfo.Admin)
            // Admins now get free accounts forever
            return false;
        if (ExpireDate() <= DateTime.UtcNow) return true;

        return false;
    }

    public DateTime ExpireDate()
    {
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();
            var x = db.Query<UserPayments>(
                "SELECT * FROM UserPayments WHERE UserId=@uid ORDER BY CreateTime DESC LIMIT 1",
                new { uid = userId });

            // var x = db.Select<Majorsilence.Vpn.Poco.UserPayments>("UserId={0}", userId);
            if (x.Count() == 0) return DateTime.UtcNow;

            var payDate = x.First().CreateTime;
            var payType = x.First().LookupPaymentTypeId;

            var expireDate = payDate;
            if (payType == SiteInfo.MonthlyPaymentId)
                expireDate = expireDate.AddMonths(1);
            else if (payType == SiteInfo.YearlyPaymentId)
                expireDate = expireDate.AddYears(1);
            else
                throw new InvalidDataException("Neither a monthly or yearly payment id has been found.");


            // Add 1 day grace to allow payment to take place
            expireDate = expireDate.AddDays(1);
            return expireDate;
        }
    }

    public void SaveUserPayment(decimal amount, DateTime createTime, int paymentCodeId)
    {
        if (userInfo == null)
            throw new InvalidUserIdException(
                string.Format("The user attempting to make payments does not exist.", userId));

        using (var db = _dbSettings.DbFactory)
        {
            db.Open();

            db.Insert(new UserPayments(
                userId, amount, createTime, paymentCodeId)
            );
        }
    }
}