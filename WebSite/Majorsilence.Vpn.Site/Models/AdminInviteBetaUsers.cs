using System.Threading.Tasks;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;

namespace Majorsilence.Vpn.Site.Models;

public class AdminInviteBetaUsers : CustomViewLayout
{
    public AdminInviteBetaUsers()
    {
        var keys = new BetaKeys(InitializeSettings.Email);
        RemainingBetaKeys = keys.UnsuedKeyCount();
    }

    public int RemainingBetaKeys { get; }

    public bool EmailSent { get; private set; }

    public async Task SendMailAsync(string emailAddress)
    {
        var keys = new BetaKeys(InitializeSettings.Email);
        await keys.MailInvite(emailAddress);
        EmailSent = true;
    }
}