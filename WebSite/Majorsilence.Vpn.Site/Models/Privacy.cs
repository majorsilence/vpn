using Majorsilence.Vpn.Logic;

namespace Majorsilence.Vpn.Site.Models;

public class Privacy
{
    public Privacy(DatabaseSettings dbSettings)
    {
        var t = new Logic.Site.Privacy(dbSettings);
        CurrentPrivacy = t.CurrentPrivacy();
    }

    public Poco.Privacy CurrentPrivacy { get; }
}