using System.Collections.Generic;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Site.Models;

public class AdminRegions : CustomViewLayout
{
    public AdminRegions()
    {
        RegionList = new Logic.Admin.Regions().Select();
    }

    public IEnumerable<Regions> RegionList { get; }
}