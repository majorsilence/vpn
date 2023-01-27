using System;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Poco;

[Table("UsersApiTokens")]
public class UsersApiTokens
{
    [Key] public int Id { get; set; }

    /// <summary>
    ///     Foreign key to users table
    /// </summary>
    /// <value>The user identifier.</value>
    public int UserId { get; set; }

    public string Token1 { get; set; }

    public DateTime Token1ExpireTime { get; set; }

    public string Token2 { get; set; }

    public DateTime Token2ExpireTime { get; set; }
}