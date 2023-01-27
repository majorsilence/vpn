using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Models;

public class AdminInviteBetaUsers : CustomViewLayout
{
    public AdminInviteBetaUsers()
    {
        var keys = new Logic.Accounts.BetaKeys(Logic.InitializeSettings.Email);
        _remainingBetaKeys = keys.UnsuedKeyCount();
    }

    private int _remainingBetaKeys;
    public int RemainingBetaKeys => _remainingBetaKeys;

    private bool _emailSent;
    public bool EmailSent => _emailSent;

    public void SendMail(string emailAddress)
    {
        var keys = new Logic.Accounts.BetaKeys(Logic.InitializeSettings.Email);
        keys.MailInvite(emailAddress);
        _emailSent = true;
    }
}