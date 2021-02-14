using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;
using Stripe;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Majorsilence.Vpn.Logic
{
    public class DailyProcessing : ICommand
    {

        public void Execute()
        {

            CheckForNewPayments();

           


        }

        /// <summary>
        /// As long as payments continue the users account will not expire
        /// </summary>
        private void CheckForNewPayments()
        {
            // retrieve all events for the past day and proceed to process 
            // subscription cancellations.

            using (var db = Setup.DbFactory)
            {

                var data = db.Query<Majorsilence.Vpn.Poco.DatabaseInfo>("SELECT * FROM DatabaseInfo");

                if (data.Count() != 1)
                {
                    throw new Majorsilence.Vpn.Logic.Exceptions.InvalidDataException("Incorrect data in DatabaseInfo table.  To many or too few rows.");
                }


                Helpers.SslSecurity.Callback();

                var eventService = new StripeChargeService(Majorsilence.Vpn.Logic.Helpers.SiteInfo.StripeAPISecretKey);
                var options = new StripeChargeListOptions()
                {
                    Limit = 1000,
                    Created = new StripeDateFilter
                    { 
                        GreaterThanOrEqual = data.First().LastDailyProcess
                    }
                };
                 

                IEnumerable<StripeCharge> response = eventService.List(options);

                foreach (var evt in response)
                {
                    //if (evt.LiveMode == false)
                    //{
                    //    continue;
                    //}
                            
                    // A new payment has been made.

                    string stripeCustomerId = evt.CustomerId;

                      
                    //var users = db.Select<Majorsilence.Vpn.Poco.Users>(q => q.PaymentExpired == false);
                    var user = db.Query<Majorsilence.Vpn.Poco.Users>("SELECT * FROM Users WHERE StripeCustomerAccount=@StripeCustomerAccount",
                                   new {StripeCustomerAccount = stripeCustomerId});

                    if (user == null || user.Count() != 1)
                    {
                         
                         

                        var ex = new Majorsilence.Vpn.Logic.Exceptions.InvalidDataException("Cannot find stripe customer data in users table.  Stripe Customer Account: " +
                                 stripeCustomerId);

                        Majorsilence.Vpn.Logic.Helpers.Logging.Log(ex);
                        Setup.Email.SendMail_BackgroundThread("Error running DailyProcessing: " + ex.Message,
                            "Error running DailyProcessing", Majorsilence.Vpn.Logic.Helpers.SiteInfo.AdminEmail, true, null, 
                            Email.EmailTemplates.Generic);

                        continue;

                    }

                    int userid = user.First().Id;
                    var pay = new Payments.Payment(userid);

                    // amount in cents
                    pay.SaveUserPayment((decimal)(evt.Amount / 100.0m), evt.Created, Helpers.SiteInfo.MonthlyPaymentId);

                    Majorsilence.Vpn.Logic.ActionLog.Log_BackgroundThread("Payment made", userid);
                }


            
           
               

                data.First().LastDailyProcess = DateTime.UtcNow;

                db.Update(data.First());

            }
           
        }
    }
}