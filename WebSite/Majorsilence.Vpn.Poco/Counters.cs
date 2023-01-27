using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Poco;

[Table("Counters")]
public class Counters
{
    public Counters()
    {
    }

    public Counters(string code, string description, ulong num)
    {
        Code = code;
        Description = description;
        Num = num;
    }

    [Key] public int Id { get; set; }

    public string Code { get; set; }

    public string Description { get; set; }

    public ulong Num { get; set; }
}