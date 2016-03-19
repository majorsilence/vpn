using System;

namespace LibLogic.Exceptions
{
    public class PasswordMismatchException : InvalidDataException
    {
        public PasswordMismatchException(string msg) : base(msg) { }

        public PasswordMismatchException(string msg, Exception ex) : base(msg, ex) { }
    }
}

