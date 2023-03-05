using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Logic.Payments;
using Majorsilence.Vpn.Poco;
using Microsoft.Extensions.Logging;
using Stripe;
using SiteInfo = Majorsilence.Vpn.Logic.Helpers.SiteInfo;

namespace Majorsilence.Vpn.Logic;

public class DailyProcessing : ICommand
{
    private readonly ILogger _logger;
    private readonly DatabaseSettings _dbSettings;
    private readonly ActionLog _actionLog;
    public DailyProcessing(ILogger logger, DatabaseSettings dbSettings,
        ActionLog actionLog)
    {
        _logger = logger;
        _dbSettings = dbSettings;
        _actionLog = actionLog;
    }
    
    public void Execute()
    {
        CheckForNewPayments();
    }

    /// <summary>
    ///     As long as payments continue the users account will not expire
    /// </summary>
    private void CheckForNewPayments()
    {
        // retrieve all events for the past day and proceed to process 
        // subscription cancellations.

        using (var db = _dbSettings.DbFactory)
        {
            var data = db.Query<DatabaseInfo>("SELECT * FROM DatabaseInfo");

            if (data.Count() != 1)
                throw new InvalidDataException(
                    "Incorrect data in DatabaseInfo table.  To many or too few rows.");


            SslSecurity.Callback();

            var client = new StripeClient(SiteInfo.StripeAPISecretKey);
            var eventService = new ChargeService(client);
            var options = new ChargeListOptions
            {
                Limit = 1000,
                Created = new AnyOf<DateTime?, DateRangeOptions>(new DateRangeOptions
                {
                    GreaterThanOrEqual = data.First().LastDailyProcess
                })
            };


            IEnumerable<Charge> response = eventService.List(options);

            foreach (var evt in response)
            {
                //if (evt.LiveMode == false)
                //{
                //    continue;
                //}

                // A new payment has been made.

                var stripeCustomerId = evt.CustomerId;


                //var users = db.Select<Majorsilence.Vpn.Poco.Users>(q => q.PaymentExpired == false);
                var user = db.Query<Users>(
                    "SELECT * FROM Users WHERE StripeCustomerAccount=@StripeCustomerAccount",
                    new { StripeCustomerAccount = stripeCustomerId });

                if (user == null || user.Count() != 1)
                {
                    _logger.LogWarning("Cannot find stripe customer data in users table.  Stripe Customer Account: " +
                                       stripeCustomerId);
                    continue;
                }

                var userid = user.First().Id;
                var pay = new Payment(userid, _dbSettings);

                // amount in cents
                pay.SaveUserPayment(evt.Amount / 100.0m, evt.Created, SiteInfo.MonthlyPaymentId);

                _actionLog.Log("Payment made", userid);
            }


            data.First().LastDailyProcess = DateTime.UtcNow;

            db.Update(data.First());
        }
    }
}