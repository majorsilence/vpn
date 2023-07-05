using Dapper;
using NUnit.Framework;

namespace Majorsilence.Vpn.Site.TestsFast.LiveSite;

public class CreateAccountTest
{
    private readonly string betaKey = "abc1";
    private readonly string emailAddress = "test@majorsilence.com";

    [TearDown]
    public void Cleanup()
    {
        using (var cn = Setup.DbSettings.DbFactory)
        {
            cn.Open();
            cn.Execute("DELETE FROM users WHERE email = @email", new { email = emailAddress });
            cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new { Code = betaKey });
        }
    }
}