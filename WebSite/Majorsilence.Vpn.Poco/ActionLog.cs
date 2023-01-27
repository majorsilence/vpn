using System;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Poco;

[Table("ActionLog")]
public class ActionLog
{
    [Key] public ulong Id { get; set; }

    public string Action { get; set; }

    public int UserId { get; set; }

    public DateTime ActionDate { get; set; }
}