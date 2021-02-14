using System;

namespace Majorsilence.Vpn.Logic.Exceptions
{
    public class InvalidBetaKeyException: Exception
    {

        public InvalidBetaKeyException(string msg) : base(msg) { }

        public InvalidBetaKeyException(string msg, Exception ex) : base(msg, ex) { }

    }
}

