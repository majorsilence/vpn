using System.Collections.Generic;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Payments;

namespace Majorsilence.Vpn.Site.Models;

public class Setup : CustomViewLayout
{
    public Setup(int userid, string username,
        DatabaseSettings dbSettings)
    {
        var details = new ServerDetails(dbSettings);
        ServerInfo = details.Info;

        var pay = new Payment(userid, dbSettings);
        ActiveAccount = !pay.IsExpired();


        var userServerDetails = new UserServerDetails(userid, dbSettings);
        if (userServerDetails.Info == null)
        {
            CurrentServer = "none";
            PptpIP = "none";
            PptpPassword = "none";
        }
        else
        {
            CurrentServer = userServerDetails.Info.VpnServerName + " - Region: " + userServerDetails.Info.RegionName;
            PptpIP = userServerDetails.Info.Address;
            PptpPassword = userServerDetails.Info.PptpPassword;
        }

        Username = username;
    }

    public string PptpIP { get; set; }

    public string PptpPassword { get; set; }

    public string CurrentServer { get; set; }

    public string Username { get; set; }

    public IEnumerable<UserServerDetailsInfo> ServerInfo { get; set; }

    public bool ActiveAccount { get; set; }
}