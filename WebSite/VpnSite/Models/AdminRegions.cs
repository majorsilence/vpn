using System;
using System.Collections.Generic;

namespace Majorsilence.Vpn.Site.Models
{
    public class AdminRegions : AdminViewLayout
    {
        public AdminRegions()
        {
            _regionList = new LibLogic.Admin.Regions().Select();
        }

        private readonly IEnumerable<LibPoco.Regions> _regionList;

        public IEnumerable<LibPoco.Regions> RegionList
        { 
            get
            {
                return _regionList;
            } 
        }
    }
}

