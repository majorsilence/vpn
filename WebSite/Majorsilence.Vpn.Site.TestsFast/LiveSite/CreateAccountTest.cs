using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Dapper;

namespace Majorsilence.Vpn.Site.TestsFast.LiveSite;

public class CreateAccountTest
{
    private readonly string emailAddress = "test@majorsilence.com";
    private readonly string betaKey = "abc1";

    [TearDown]
    public void Cleanup()
    {
        using (var cn = Logic.InitializeSettings.DbFactory)
        {
            cn.Open();
            cn.Execute("DELETE FROM users WHERE email = @email", new { email = emailAddress });
            cn.Execute("UPDATE BetaKeys SET IsUsed = 0 WHERE Code = @Code", new { Code = betaKey });
        }
    }
}