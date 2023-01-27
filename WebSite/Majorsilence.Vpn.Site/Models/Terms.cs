using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Site.Models;

public class Terms
{
    public Terms()
    {
        var t = new Logic.Site.TermsOfService();
        TOS = t.CurrentTermsOfService();
    }

    public TermsOfService TOS { get; }
}