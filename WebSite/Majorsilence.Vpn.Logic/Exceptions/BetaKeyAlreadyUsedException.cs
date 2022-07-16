using System;

namespace Majorsilence.Vpn.Logic.Exceptions
{
    public class BetaKeyAlreadyUsedException: InvalidDataException
    {
        public BetaKeyAlreadyUsedException(string msg) : base(msg) { }

        public BetaKeyAlreadyUsedException(string msg, Exception ex) : base(msg, ex) { }
    }
}

