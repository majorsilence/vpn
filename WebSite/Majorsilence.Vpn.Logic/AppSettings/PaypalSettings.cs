using Majorsilence.Vpn.Logic.Payments;

namespace Majorsilence.Vpn.Logic.AppSettings;

public class PaypalSettings : IPaypalSettings
{
    public string Username { get; init; }
    public string Password { get; init; }
    public string Url { get; init; }
}