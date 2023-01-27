using System;

namespace Majorsilence.Vpn.Site.Models;

public class Privacy
{
    public Privacy()
    {
        var t = new Majorsilence.Vpn.Logic.Site.Privacy();
        _priv = t.CurrentPrivacy();
    }

    private readonly Majorsilence.Vpn.Poco.Privacy _priv;
    public Majorsilence.Vpn.Poco.Privacy CurrentPrivacy => _priv;
}