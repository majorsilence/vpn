using System;
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

            var t = new LibLogic.Site.TermsOfService();
            _terms = t.CurrentTermsOfService();
        }

        private LibPoco.TermsOfService _terms;
        public LibPoco.TermsOfService TOS
        {
            get
            {
                return _terms;
            }
        }


    }
}

