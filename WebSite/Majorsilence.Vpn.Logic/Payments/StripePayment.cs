using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Poco;
using Stripe;
using SiteInfo = Majorsilence.Vpn.Logic.Helpers.SiteInfo;

namespace Majorsilence.Vpn.Logic.Payments;

public class StripePayment
{
    private readonly DatabaseSettings _dbSettings;
    private readonly int _userId;
    private readonly IEmail email;

    private StripePayment()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="userId"></param>
    public StripePayment(int userId, IEmail email, DatabaseSettings dbSettings)
    {
        _userId = userId;
        this.email = email;
        _dbSettings = dbSettings;
    }


    public void MakePayment(string stripeToken, string coupon)
    {
        SslSecurity.Callback();

        if (stripeToken.Trim() == "")
            throw new InvalidStripeTokenException("Making a payment requires a stripetoken");

        var customerDetails = GetSavedCustomerStripeDetails();
        if (customerDetails == null)
            CreateStripeCustomer(stripeToken, coupon);
        else if (string.IsNullOrWhiteSpace(customerDetails.Value.StripeSubscriptionId))
            CreateStripeSubscription(stripeToken, coupon);

        var pay = new Payment(_userId, _dbSettings);
        pay.SaveUserPayment(SiteInfo.CurrentMonthlyRate, DateTime.UtcNow,
            SiteInfo.MonthlyPaymentId);
    }

    public async Task CancelAccount()
    {
        SslSecurity.Callback();

        var customerDetails = GetSavedCustomerStripeDetails();
        if (customerDetails == null)
            throw new InvalidDataException(
                "Attempting to cancel an account but the customer does not have any stripe details");

        var client = new StripeClient(SiteInfo.StripeAPISecretKey);
        var custService = new CustomerService(client);
        custService.Delete(customerDetails.Value.StripeCustId);

        using (var db = _dbSettings.DbFactory)
        {
            var data = db.Get<Users>(_userId);
            data.StripeCustomerAccount = "";
            db.Update(data);

            await email.SendMail("Your vpn credit card account has been deleted.  " +
                                 "You will not be billed again.  You will continue to have access until your current payment expires.",
                "VPN Credit Card Account Deleted", data.Email, true, null,
                EmailTemplates.Generic);
        }
    }

    public async Task CancelSubscription()
    {
        SslSecurity.Callback();

        var customerDetails = GetSavedCustomerStripeDetails();
        if (customerDetails.HasValue)
        {
            var client = new StripeClient(SiteInfo.StripeAPISecretKey);
            var subscriptionService = new SubscriptionService(client);
            subscriptionService.Cancel(customerDetails.Value.StripeSubscriptionId);
        }

        using (var db = _dbSettings.DbFactory)
        {
            var data = db.Get<Users>(_userId);
            data.StripeSubscriptionId = "";
            db.Update(data);

            await email.SendMail("Your vpn account subscription has been cancelled.  " +
                                 "You will not be billed again.  You will continue to have access until your current payment expires.",
                "VPN Account Subscription Cancelled", data.Email, true, null,
                EmailTemplates.Generic);
        }

        if (customerDetails == null)
            throw new InvalidDataException(
                "Attempting to cancel an account but the customer does not have any stripe details.  Only removed from database.  Nothing removed from stripe.");
    }

    private void CreateStripeSubscription(string stripeToken, string coupon)
    {
        using (var db = _dbSettings.DbFactory)
        {
            var data = db.Get<Users>(_userId);

            var subscriptionService = new SubscriptionService();
            var stripeSubscription = subscriptionService.Create(new SubscriptionCreateOptions
            {
                Customer = data.StripeCustomerAccount,
                Items = new List<SubscriptionItemOptions>
                {
                    new()
                    {
                        Price = SiteInfo.StripePlanId
                    }
                },
                Coupon = string.IsNullOrWhiteSpace(coupon) ? null : coupon
            });

            var client = new StripeClient(SiteInfo.StripeAPISecretKey);
            var subscriptionInfo = new SubscriptionService(client);

            var options = new SubscriptionListOptions
            {
                Limit = 3
            };
            var subscriptionList = subscriptionInfo.List(new SubscriptionListOptions
            {
                Customer = data.StripeCustomerAccount
            }).ToList();

            if (subscriptionList.Count() > 1)
                throw new StripeSubscriptionException(
                    string.Format("More then one subscription detected for vpn customer: {0}, stripe customer: {1}",
                        _userId, data.StripeCustomerAccount)
                );


            data.StripeSubscriptionId = subscriptionList.First().Id;

            db.Update(data);
        }
    }

    private void CreateStripeCustomer(string stripeToken, string coupon)
    {
        var customer = new CustomerCreateOptions();
        using (var db = _dbSettings.DbFactory)
        {
            var data = db.Get<Users>(_userId);


            // If it is the first time the customer has paid we have not created an account yet
            // so do it now.
            customer.Email = data.Email;
            customer.Description = string.Format("{0} {1} ({2})", data.FirstName, data.LastName, data.Email);
            customer.Plan = SiteInfo.StripePlanId;
            //customer.TrialEnd = DateTime.Now.AddDays(30);
            if (coupon.Trim() != "") customer.Coupon = coupon;

            var client = new StripeClient(SiteInfo.StripeAPISecretKey);
            var customerService = new CustomerService(client);
            var cust = customerService.Create(customer);

            var subscriptionInfo = new SubscriptionService(client);
            var subscriptionList = subscriptionInfo.List(new SubscriptionListOptions { Customer = cust.Id })
                .ToList();

            if (subscriptionList.Count() > 1)
                throw new StripeSubscriptionException(
                    string.Format("More then one subscription detected for vpn customer: {0}, stripe customer: {1}",
                        _userId, cust.Id)
                );

            data.StripeCustomerAccount = cust.Id;
            data.StripeSubscriptionId = subscriptionList.First().Id;


            db.Update(data);
        }
    }


    /// <summary>
    ///     Retrieve the customers stripe customer id and customers stripe subscription id.  If the user
    ///     does not have a stripe customer id in the users
    ///     table a new stripe customer is create using the token
    ///     and the users table is updated with the new customer id.
    /// </summary>
    /// <param name="stripeToken"></param>
    /// <param name="coupon"></param>
    /// <returns></returns>
    private (string StripeCustId, string StripeSubscriptionId)? GetSavedCustomerStripeDetails()
    {
        var stripeCustId = "";
        var stripeSubscriptionId = "";

        using (var db = _dbSettings.DbFactory)
        {
            var data = db.Get<Users>(_userId);

            stripeCustId = data.StripeCustomerAccount;
            stripeSubscriptionId = data.StripeSubscriptionId;
        }


        if (stripeCustId.Trim() == "") return null;

        return (stripeCustId, stripeSubscriptionId);
    }
}