using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibLogic.Exceptions
{
    public class SshException : Exception
    {

        public SshException(string msg) : base(msg) { }

        public SshException(string msg, Exception ex) : base(msg, ex) { }

    }
}