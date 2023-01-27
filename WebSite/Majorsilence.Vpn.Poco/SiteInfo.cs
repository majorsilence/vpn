using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Majorsilence.Vpn.Poco;

[Dapper.Contrib.Extensions.Table("SiteInfo")]
public class SiteInfo
{
    public SiteInfo()
    {
    }

    public SiteInfo(string vpnSshUser, string vpnSshPassword, int sshPort,
        string adminEmail, string siteName, string siteUrl, string stripeApiSecretKey,
        string stripeAPIPublicKey,
        bool liveSite, string stripePlanId, string currency)
    {
        VpnSshUser = vpnSshUser;
        VpnSshPassword = vpnSshPassword;
        SshPort = sshPort;
        AdminEmail = adminEmail;
        SiteName = siteName;
        SiteUrl = siteUrl;
        StripeAPISecretKey = stripeApiSecretKey;
        StripeAPIPublicKey = stripeAPIPublicKey;
        LiveSite = liveSite;
        StripePlanId = stripePlanId;
        Currency = currency;
    }

    [Dapper.Contrib.Extensions.Key()] public int Id { get; set; }

    public string VpnSshUser { get; set; }

    public string VpnSshPassword { get; set; }

    public int SshPort { get; set; }

    public string AdminEmail { get; set; }

    public string SiteName { get; set; }

    public string SiteUrl { get; set; }

    public string StripeAPISecretKey { get; set; }

    public string StripeAPIPublicKey { get; set; }

    public bool LiveSite { get; set; }

    public string StripePlanId { get; set; }

    public string Currency { get; set; }
}