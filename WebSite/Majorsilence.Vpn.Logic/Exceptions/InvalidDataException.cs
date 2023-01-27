using System;

namespace Majorsilence.Vpn.Logic.Exceptions;

public class InvalidDataException : Exception
{
    public InvalidDataException(string msg) : base(msg)
    {
    }

    public InvalidDataException(string msg, Exception ex) : base(msg, ex)
    {
    }
}