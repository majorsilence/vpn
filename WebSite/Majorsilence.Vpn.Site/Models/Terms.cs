﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Models
{
    public class Terms
    {

        public Terms()
        {

            var t = new Majorsilence.Vpn.Logic.Site.TermsOfService();
            _terms = t.CurrentTermsOfService();
        }

        private Majorsilence.Vpn.Poco.TermsOfService _terms;
        public Majorsilence.Vpn.Poco.TermsOfService TOS
        {
            get
            {
                return _terms;
            }
        }


    }
}

