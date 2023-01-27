using System;

namespace Majorsilence.Vpn.Logic.Exceptions;

public class PasswordLengthException : InvalidDataException
{
    public PasswordLengthException(string msg) : base(msg)
    {
    }

    public PasswordLengthException(string msg, Exception ex) : base(msg, ex)
    {
    }
}