using System;
using System.Collections.Generic;

namespace Majorsilence.Vpn.Site.Models;

public class Setup : CustomViewLayout
{
    public Setup(int userid, string username)
    {
        var details = new Logic.Accounts.ServerDetails();
        ServerInfo = details.Info;

        var pay = new Logic.Payments.Payment(userid);
        ActiveAccount = !pay.IsExpired();


        var userServerDetails = new Logic.Accounts.UserServerDetails(userid);
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

    public IEnumerable<Logic.Accounts.UserServerDetailsInfo> ServerInfo { get; set; }

    public bool ActiveAccount { get; set; }
}