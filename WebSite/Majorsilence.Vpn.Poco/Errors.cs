using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Majorsilence.Vpn.Poco;

[Dapper.Contrib.Extensions.Table("Errors")]
public class Errors
{
    public Errors()
    {
    }

    public Errors(DateTime timeCreated, string message, string stackTrace, string recursiveStackTrace)
    {
        TimeCreated = timeCreated;
        Message = message;
        StackTrace = stackTrace;
        RecursiveStackTrace = recursiveStackTrace;
    }

    [Dapper.Contrib.Extensions.Key()] public int Id { get; set; }

    public DateTime TimeCreated { get; set; }

    public string Message { get; set; }

    public string StackTrace { get; set; }

    public string RecursiveStackTrace { get; set; }
}