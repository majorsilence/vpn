using System;

namespace LibLogic.Exceptions
{
    public class InvalidBetaKeyException: Exception
    {

        public InvalidBetaKeyException(string msg) : base(msg) { }

        public InvalidBetaKeyException(string msg, Exception ex) : base(msg, ex) { }

    }
}

