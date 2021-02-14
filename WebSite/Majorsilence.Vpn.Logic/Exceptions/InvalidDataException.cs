using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibLogic.Exceptions
{
    public class InvalidDataException: Exception
    {

        public InvalidDataException(string msg) : base(msg) { }

        public InvalidDataException(string msg, Exception ex) : base(msg, ex) { }

    }
}