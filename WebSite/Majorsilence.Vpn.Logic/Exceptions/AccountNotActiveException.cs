using System;

namespace Majorsilence.Vpn.Logic.Exceptions;

public class AccountNotActiveException : Exception
{
    public AccountNotActiveException()
    {
    }

    public AccountNotActiveException(string msg) : base(msg)
    {
    }

    public AccountNotActiveException(string msg, Exception ex) : base(msg, ex)
    {
    }
}