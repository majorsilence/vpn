using Majorsilence.Vpn.Site.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Majorsilence.Vpn.Site.Models
{
    public class CustomViewLayout
    {
        public CustomViewLayout(ISessionVariables sessionVars)
        {
            SessionVariables = sessionVars;
        }
        public CustomViewLayout() { }
        public bool IsAdmin { get { return SessionVariables.IsAdmin; } }
        public ISessionVariables SessionVariables { get; set; }
    }
}
