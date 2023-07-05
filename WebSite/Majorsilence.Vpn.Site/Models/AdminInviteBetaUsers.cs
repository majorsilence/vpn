using System.Threading.Tasks;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;
using Majorsilence.Vpn.Logic.Email;

namespace Majorsilence.Vpn.Site.Models;

public class AdminInviteBetaUsers : CustomViewLayout
{
    private readonly DatabaseSettings _dbSettings;
    private readonly IEmail _email;

    public AdminInviteBetaUsers(IEmail email, DatabaseSettings dbSettings)
    {
        _email = email;
        _dbSettings = dbSettings;
        var keys = new BetaKeys(_email, _dbSettings);
        RemainingBetaKeys = keys.UnsuedKeyCount();
    }

    public int RemainingBetaKeys { get; }

    public bool EmailSent { get; private set; }

    public async Task SendMailAsync(string emailAddress)
    {
        var keys = new BetaKeys(_email, _dbSettings);
        await keys.MailInvite(emailAddress);
        EmailSent = true;
    }
}