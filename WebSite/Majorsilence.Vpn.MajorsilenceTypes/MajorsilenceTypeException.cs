using System;

namespace MajorsilenceTypes;

public class MajorsilenceTypeException : Exception
{
    /// <summary>
    /// </summary>
    /// <param name="msg"></param>
    public MajorsilenceTypeException(string msg)
        : base(msg)
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="ex"></param>
    public MajorsilenceTypeException(string msg, Exception ex)
        : base(msg, ex)
    {
    }
}