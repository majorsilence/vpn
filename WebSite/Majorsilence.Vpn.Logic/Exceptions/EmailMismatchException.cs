using System;

namespace Majorsilence.Vpn.Logic.Exceptions;

public class EmailMismatchException : InvalidDataException
{
    public EmailMismatchException(string msg) : base(msg)
    {
    }

    public EmailMismatchException(string msg, Exception ex) : base(msg, ex)
    {
    }
}