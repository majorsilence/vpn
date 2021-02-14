using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;
using Stripe;

namespace Majorsilence.Vpn.Logic.Payments
{
    public class StripePayment
    {
        private StripePayment()
        {
        }

        private int _userId;
        private  Majorsilence.Vpn.Logic.Email.IEmail email;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        public StripePayment(int userId, Majorsilence.Vpn.Logic.Email.IEmail email)
        {
            _userId = userId;
            this.email = email;
        }


        public void MakePayment(string stripeToken, string coupon)
        {
            Helpers.SslSecurity.Callback();

            if (stripeToken.Trim() == "")
            {
                throw new Exceptions.InvalidStripeTokenException("Making a payment requires a stripetoken");
            }

            var customerDetails = GetSavedCustomerStripeDetails();
            if (customerDetails == null)
            {
                CreateStripeCustomer(stripeToken, coupon);
            }
            else if (customerDetails.Item2.Trim() == "")
            {
                CreateStripeSubscription(stripeToken, coupon);
            }
                
            var pay = new Payment(this._userId);
            pay.SaveUserPayment(Helpers.SiteInfo.CurrentMonthlyRate, DateTime.UtcNow, Helpers.SiteInfo.MonthlyPaymentId);
            
        }

        public void CancelAccount()
        {

            Helpers.SslSecurity.Callback();

            var customerDetails = GetSavedCustomerStripeDetails();
            if (customerDetails == null)
            {
                throw new Exceptions.InvalidDataException("Attempting to cancel an account but the customer does not have any stripe details");
            }

            var custService = new StripeCustomerService(Helpers.SiteInfo.StripeAPISecretKey);
            custService.Delete(customerDetails.Item1);    

            using (var db = Setup.DbFactory)
            {

                var data = db.Get<Majorsilence.Vpn.Poco.Users>(_userId);
                data.StripeCustomerAccount = "";
                db.Update(data);

                this.email.SendMail_BackgroundThread("Your vpn credit card account has been deleted.  " +
                "You will not be billed again.  You will continue to have access until your current payment expires.", 
                    "VPN Credit Card Account Deleted", data.Email, true, null, Majorsilence.Vpn.Logic.Email.EmailTemplates.Generic);

            }


        }

        public void CancelSubscription()
        {
            Helpers.SslSecurity.Callback();

            var customerDetails = GetSavedCustomerStripeDetails();
            if (customerDetails != null)
            {
                var subscriptionService = new StripeSubscriptionService(Helpers.SiteInfo.StripeAPISecretKey);
                subscriptionService.Cancel(customerDetails.Item1, customerDetails.Item2);    
            }

            using (var db = Setup.DbFactory)
            {

                var data = db.Get<Majorsilence.Vpn.Poco.Users>(_userId);
                data.StripeSubscriptionId = "";
                db.Update(data);

                this.email.SendMail_BackgroundThread("Your vpn account subscription has been cancelled.  " +
                "You will not be billed again.  You will continue to have access until your current payment expires.", 
                    "VPN Account Subscription Cancelled", data.Email, true, null, Majorsilence.Vpn.Logic.Email.EmailTemplates.Generic);

            }

            if (customerDetails == null)
            {
                throw new Exceptions.InvalidDataException("Attempting to cancel an account but the customer does not have any stripe details.  Only removed from database.  Nothing removed from stripe.");
            }

        }

        private void CreateStripeSubscription(string stripeToken, string coupon)
        {
 
            using (var db = Setup.DbFactory)
            {

                var data = db.Get<Majorsilence.Vpn.Poco.Users>(_userId);


                var options = new Stripe.StripeSubscriptionCreateOptions();
                options.TokenId = stripeToken;
                if (coupon.Trim() != "")
                {
                    options.CouponId = coupon;
                }

                var subscriptionService = new StripeSubscriptionService();
                StripeSubscription stripeSubscription = subscriptionService.Create(data.StripeCustomerAccount, 
                                                            Helpers.SiteInfo.StripePlanId, options);


                var subscriptionInfo = new StripeSubscriptionService(Helpers.SiteInfo.StripeAPISecretKey);
                var subscriptionList = subscriptionInfo.List(data.StripeCustomerAccount).ToList();

                if (subscriptionList.Count() > 1)
                {
                    throw new Exceptions.StripeSubscriptionException(
                        string.Format("More then one subscription detected for vpn customer: {0}, stripe customer: {1}", 
                            _userId, data.StripeCustomerAccount)
                    );
                }

        
          
               
                data.StripeSubscriptionId = subscriptionList.First().Id;
               
                db.Update(data);

            }
                
        }

        private void CreateStripeCustomer(string stripeToken, string coupon)
        {

            var customer = new Stripe.StripeCustomerCreateOptions();
            using (var db = Setup.DbFactory)
            {

                var data = db.Get<Majorsilence.Vpn.Poco.Users>(_userId);


                // If it is the first time the customer has paid we have not created an account yet
                // so do it now.
                customer.Email = data.Email;
                customer.Description = string.Format("{0} {1} ({2})", data.FirstName, data.LastName, data.Email);
                customer.TokenId = stripeToken;
                customer.PlanId = Helpers.SiteInfo.StripePlanId;
                //customer.TrialEnd = DateTime.Now.AddDays(30);
                if (coupon.Trim() != "")
                {
                    customer.CouponId = coupon;
                }
                var customerService = new StripeCustomerService(Helpers.SiteInfo.StripeAPISecretKey);
                var cust = customerService.Create(customer);

                var subscriptionInfo = new StripeSubscriptionService(Helpers.SiteInfo.StripeAPISecretKey);
                var subscriptionList = subscriptionInfo.List(cust.Id).ToList();

                if (subscriptionList.Count() > 1)
                {
                    throw new Exceptions.StripeSubscriptionException(
                        string.Format("More then one subscription detected for vpn customer: {0}, stripe customer: {1}", _userId, cust.Id)
                    );
                }

                data.StripeCustomerAccount = cust.Id;
                data.StripeSubscriptionId = subscriptionList.First().Id;



                db.Update(data);


            }
                

        }


        /// <summary>
        /// Retrieve the customers stripe customer id and customers stripe subscription id.  If the user
        /// does not have a stripe customer id in the users
        /// table a new stripe customer is create using the token
        ///  and the users table is updated with the new customer id.
        /// </summary>
        /// <param name="stripeToken"></param>
        /// <param name="coupon"></param>
        /// <returns></returns>
        private Tuple<string, string> GetSavedCustomerStripeDetails()
        {

            string stripeCustId = "";
            string stripeSubscriptionId = "";
    
            using (var db = Setup.DbFactory)
            {

                var data = db.Get<Majorsilence.Vpn.Poco.Users>(_userId);

                stripeCustId = data.StripeCustomerAccount;
                stripeSubscriptionId = data.StripeSubscriptionId;


            }


            if (stripeCustId.Trim() == "")
            {
                return null;
            }

            return Tuple.Create<string, string>(stripeCustId, stripeSubscriptionId);

        }

       


    }
}