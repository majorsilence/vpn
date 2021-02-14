using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibLogic.Payments
{
    public class StripWebHook : ICommand
    {
        private StripWebHook() { }

        string json;
        public StripWebHook(string json)
        {
            this.json = json;



        }

        public void Execute()
        {
            var stripeEvent = StripeEventUtility.ParseEvent(json);

            if (stripeEvent.LiveMode == false)
            {
                return;
            }


            switch (stripeEvent.Type)
            {
                case "customer.subscription.deleted":
                    // Event fires when a user cancels a subscription
                    var aa = Stripe.Mapper<StripeSubscription>.MapFromJson(stripeEvent.Data.Object.ToString());
                    var customerId = aa.CustomerId;
                    break;
                case "charge.refunded":  // take a look at all the types here: https://stripe.com/docs/api#event_types
                    var stripeCharge = Stripe.Mapper<StripeCharge>.MapFromJson(stripeEvent.Data.Object.ToString());
                    break;
            }


            // TODO: Actually handle the hook if LibLogic.DailyProcessing is not being used

        }


    }
}
