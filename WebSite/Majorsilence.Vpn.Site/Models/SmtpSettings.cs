using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VpnSite.Models;

public class SmtpSettings
{
    public string FromAddress { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
}