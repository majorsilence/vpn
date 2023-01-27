using System;

namespace Majorsilence.Vpn.Logic.Exceptions;

public class EmailAddressAlreadyUsedException : Exception
{
    public EmailAddressAlreadyUsedException(string msg) : base(msg)
    {
    }

    public EmailAddressAlreadyUsedException(string msg, Exception ex) : base(msg, ex)
    {
    }
}