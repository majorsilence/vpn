using System;

namespace MajorsilenceTypes;

public class LengthException : MajorsilenceTypeException
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="msg"></param>
    public LengthException(string msg)
        : base(msg)
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="ex"></param>
    public LengthException(string msg, Exception ex)
        : base(msg, ex)
    {
    }
}