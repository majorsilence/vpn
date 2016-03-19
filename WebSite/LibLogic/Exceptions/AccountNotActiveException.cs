using System;

namespace LibLogic.Exceptions
{
    public class AccountNotActiveException : Exception
    {
        public AccountNotActiveException() : base ()
        {
        }

        public AccountNotActiveException(string msg) : base(msg)
        {
        }

        public AccountNotActiveException(string msg, Exception ex) : base(msg, ex)
        {
        }
    }
}

