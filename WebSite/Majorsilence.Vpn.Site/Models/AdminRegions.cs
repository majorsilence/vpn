using System;
using System.Collections.Generic;

namespace Majorsilence.Vpn.Site.Models;

public class AdminRegions : CustomViewLayout
{
    public AdminRegions()
    {
        _regionList = new Logic.Admin.Regions().Select();
    }

    private readonly IEnumerable<Poco.Regions> _regionList;

    public IEnumerable<Poco.Regions> RegionList => _regionList;
}