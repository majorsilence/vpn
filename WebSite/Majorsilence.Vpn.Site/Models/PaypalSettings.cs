using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Majorsilence.Vpn.Logic.Payments;

namespace VpnSite.Models
{
    public class PaypalSettings : IPaypalSettings
    {
        public string Username { get; init; }
        public string Password { get; init; }
        public string Url { get; init; }
    }
}