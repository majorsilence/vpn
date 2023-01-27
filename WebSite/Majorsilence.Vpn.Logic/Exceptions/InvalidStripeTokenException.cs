using System;

namespace Majorsilence.Vpn.Logic.Exceptions;

public class InvalidStripeTokenException : Exception
{
    public InvalidStripeTokenException()
    {
    }

    public InvalidStripeTokenException(string msg) : base(msg)
    {
    }

    public InvalidStripeTokenException(string msg, Exception ex) : base(msg, ex)
    {
    }
}