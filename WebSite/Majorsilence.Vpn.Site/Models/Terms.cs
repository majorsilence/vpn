using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Models;

public class Terms
{
    public Terms()
    {
        var t = new Majorsilence.Vpn.Logic.Site.TermsOfService();
        _terms = t.CurrentTermsOfService();
    }

    private Poco.TermsOfService _terms;
    public Poco.TermsOfService TOS => _terms;
}