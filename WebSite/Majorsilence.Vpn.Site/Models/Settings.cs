using Majorsilence.Vpn.Logic.Payments;
using VpnSite.Models;

namespace Majorsilence.Vpn.Site.Models;

public class Settings
{
    public IPaypalSettings Paypal { get; init; }
    public SmtpSettings Smtp { get; init; }
    public EncryptionKeysSettings EncryptionKeys { get; init; } 
}