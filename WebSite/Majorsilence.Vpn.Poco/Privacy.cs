using System;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Poco;

[Table("Privacy")]
public class Privacy
{
    [Key] public int Id { get; set; }

    public string Policy { get; set; }

    public DateTime CreateTime { get; set; }
}