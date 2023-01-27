using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Poco;

[Table("LookupPaymentType")]
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

    [Key] public int Id { get; set; }

    public string Code { get; set; }

    public string Description { get; set; }
}