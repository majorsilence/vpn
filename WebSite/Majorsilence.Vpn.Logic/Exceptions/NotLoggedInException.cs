﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibLogic.Exceptions
{
    public class NotLoggedInException: Exception
    {

        public NotLoggedInException(string msg) : base(msg) { }

        public NotLoggedInException(string msg, Exception ex) : base(msg, ex) { }

    }
}