using System;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Poco;

[Table("TermsOfService")]
public class TermsOfService
{
    [Key] public int Id { get; set; }

    public string Terms { get; set; }

    public DateTime CreateTime { get; set; }
}