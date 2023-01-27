using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Majorsilence.Vpn.Logic.Payments;

public class StripWebHook : ICommand
{
    private StripWebHook()
    {
    }

    private string json;

    public StripWebHook(string json)
    {
        this.json = json;
    }

    public void Execute()
    {
        var stripeEvent = Stripe.EventUtility.ParseEvent(json);

        if (stripeEvent.Livemode == false) return;


        switch (stripeEvent.Type)
        {
            case "customer.subscription.deleted":
                // Event fires when a user cancels a subscription
                var aa = (Stripe.Subscription)stripeEvent.Data.Object;
                var customerId = aa.CustomerId;
                break;
            case "charge.refunded": // take a look at all the types here: https://stripe.com/docs/api#event_types
                var stripeCharge = (Stripe.Charge)stripeEvent.Data.Object;
                break;
        }


        // TODO: Actually handle the hook if Majorsilence.Vpn.Logic.DailyProcessing is not being used
    }
}