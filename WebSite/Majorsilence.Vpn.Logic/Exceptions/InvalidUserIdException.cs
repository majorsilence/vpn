using System;

namespace Majorsilence.Vpn.Logic.Exceptions
{
    public class InvalidUserIdException : InvalidDataException
    {

        public InvalidUserIdException(string msg) : base(msg) { }

        public InvalidUserIdException(string msg, Exception ex) : base(msg, ex) { }

    }
}

