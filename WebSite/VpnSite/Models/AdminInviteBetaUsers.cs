using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Models
{
    public class AdminInviteBetaUsers
    {
        public AdminInviteBetaUsers()
        {
            var keys = new LibLogic.Accounts.BetaKeys(LibLogic.Setup.Email);
            _remainingBetaKeys = keys.UnsuedKeyCount();
        }

        private int _remainingBetaKeys;
        public int RemainingBetaKeys
        {
            get
            {
                return _remainingBetaKeys;
            }
        }

        private bool _emailSent;
        public bool EmailSent
        {
            get
            {
                return _emailSent;
            }
        }

        public void SendMail(string emailAddress)
        {
            var keys = new LibLogic.Accounts.BetaKeys(LibLogic.Setup.Email);
            keys.MailInvite(emailAddress);
            _emailSent = true;

        }


    }
}

