using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Majorsilence.Vpn.Poco;

[Dapper.Contrib.Extensions.Table("LookupPaymentType")]
public class LookupPaymentType
{
    public LookupPaymentType()
    {
    }

    public LookupPaymentType(string code, string description)
    {
        Code = code;
        Description = description;
    }

    [Dapper.Contrib.Extensions.Key()] public int Id { get; set; }

    public string Code { get; set; }

    public string Description { get; set; }
}