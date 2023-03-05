using System.Collections.Generic;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Site.Models;

public class AdminRegions : CustomViewLayout
{
    public AdminRegions(DatabaseSettings dbSettings)
    {
        RegionList = new Logic.Admin.Regions(dbSettings).Select();
    }

    public IEnumerable<Regions> RegionList { get; }
}