using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Site.Models;

public class Terms
{
    public Terms(DatabaseSettings dbSettings)
    {
        var t = new Logic.Site.TermsOfService(dbSettings);
        TOS = t.CurrentTermsOfService();
    }

    public TermsOfService TOS { get; }
}