using Majorsilence.Vpn.Site.Helpers;

namespace Majorsilence.Vpn.Site.Models;

public class CustomViewLayout
{
    public CustomViewLayout(ISessionVariables sessionVars)
    {
        SessionVariables = sessionVars;
    }

    public CustomViewLayout()
    {
    }

    public bool IsAdmin => SessionVariables.IsAdmin;
    public ISessionVariables SessionVariables { get; set; }
}