using System;

namespace Majorsilence.Vpn.Logic.Exceptions;

public class SshException : Exception
{
    public SshException(string msg) : base(msg)
    {
    }

    public SshException(string msg, Exception ex) : base(msg, ex)
    {
    }
}