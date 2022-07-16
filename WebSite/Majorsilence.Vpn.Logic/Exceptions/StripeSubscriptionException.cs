using System;

namespace Majorsilence.Vpn.Logic.Exceptions
{
    public class StripeSubscriptionException : Exception
    {

        public StripeSubscriptionException(string msg) : base(msg)
        {
        }

        public StripeSubscriptionException(string msg, Exception ex) : base(msg, ex)
        {
        }

    }
}

