using System;
using System.Collections.Generic;

namespace VpnSite.Models
{
    public class Setup
    {
        public Setup()
        {
            var details = new LibLogic.Accounts.ServerDetails();
            this.ServerInfo = details.Info;

            var pay = new LibLogic.Payments.Payment(Helpers.SessionVariables.Instance.UserId);
            ActiveAccount = !pay.IsExpired();


            var userServerDetails = new LibLogic.Accounts.UserServerDetails(Helpers.SessionVariables.Instance.UserId);
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

            Username = Helpers.SessionVariables.Instance.Username;
        }

        public string PptpIP { get; set; }

        public string PptpPassword { get; set; }

        public string CurrentServer { get; set; }

        public string Username { get; set; }

        public IEnumerable<LibLogic.Accounts.UserServerDetailsInfo> ServerInfo { get; set; }

        public bool ActiveAccount { get; set; }
    }
}

