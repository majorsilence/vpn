﻿using System;

namespace Majorsilence.Vpn.Logic.Exceptions;

public class NotLoggedInException : Exception
{
    public NotLoggedInException(string msg) : base(msg)
    {
    }

    public NotLoggedInException(string msg, Exception ex) : base(msg, ex)
    {
    }
}