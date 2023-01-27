using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Poco;

[Table("BetaKeys")]
public class BetaKeys
{
    public BetaKeys()
    {
    }

    public BetaKeys(string code, bool isUsed, bool isSent)
    {
        Code = code;
        IsUsed = isUsed;
        IsSent = isSent;
    }

    [Key] public int Id { get; set; }

    public string Code { get; set; }

    public bool IsUsed { get; set; }

    public bool IsSent { get; set; }
}