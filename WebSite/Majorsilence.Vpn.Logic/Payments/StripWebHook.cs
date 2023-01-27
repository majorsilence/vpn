using Stripe;

namespace Majorsilence.Vpn.Logic.Payments;

public class StripWebHook : ICommand
{
    private readonly string json;

    private StripWebHook()
    {
    }

    public StripWebHook(string json)
    {
        this.json = json;
    }

    public void Execute()
    {
        var stripeEvent = EventUtility.ParseEvent(json);

        if (stripeEvent.Livemode == false) return;


        switch (stripeEvent.Type)
        {
            case "customer.subscription.deleted":
                // Event fires when a user cancels a subscription
                var aa = (Subscription)stripeEvent.Data.Object;
                var customerId = aa.CustomerId;
                break;
            case "charge.refunded": // take a look at all the types here: https://stripe.com/docs/api#event_types
                var stripeCharge = (Charge)stripeEvent.Data.Object;
                break;
        }


        // TODO: Actually handle the hook if Majorsilence.Vpn.Logic.DailyProcessing is not being used
    }
}