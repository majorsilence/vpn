using Majorsilence.Vpn.Logic.Payments;

namespace Majorsilence.Vpn.Logic.AppSettings;

public class Settings
{
    public IPaypalSettings Paypal { get; init; }
    public SmtpSettings Smtp { get; init; }
    public EncryptionKeysSettings EncryptionKeys { get; init; } 
}