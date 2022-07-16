using System;
using System.Collections.Generic;

namespace Majorsilence.Vpn.Site.Models
{
    public class AdminRegions : CustomViewLayout
    {
        public AdminRegions()
        {
            _regionList = new Majorsilence.Vpn.Logic.Admin.Regions().Select();
        }

        private readonly IEnumerable<Majorsilence.Vpn.Poco.Regions> _regionList;

        public IEnumerable<Majorsilence.Vpn.Poco.Regions> RegionList
        { 
            get
            {
                return _regionList;
            } 
        }
    }
}

