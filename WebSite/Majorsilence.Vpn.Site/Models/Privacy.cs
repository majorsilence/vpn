namespace Majorsilence.Vpn.Site.Models;

public class Privacy
{
    public Privacy()
    {
        var t = new Logic.Site.Privacy();
        CurrentPrivacy = t.CurrentPrivacy();
    }

    public Poco.Privacy CurrentPrivacy { get; }
}