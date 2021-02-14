using System;
using System.Collections.Generic;

namespace Majorsilence.Vpn.Site.Models
{
    public class Setup : CustomViewLayout
    {
        public Setup(int userid, string username)
        {
            var details = new Majorsilence.Vpn.Logic.Accounts.ServerDetails();
            this.ServerInfo = details.Info;

            var pay = new Majorsilence.Vpn.Logic.Payments.Payment(userid);
            ActiveAccount = !pay.IsExpired();


            var userServerDetails = new Majorsilence.Vpn.Logic.Accounts.UserServerDetails(userid);
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

        public IEnumerable<Majorsilence.Vpn.Logic.Accounts.UserServerDetailsInfo> ServerInfo { get; set; }

        public bool ActiveAccount { get; set; }
    }
}

