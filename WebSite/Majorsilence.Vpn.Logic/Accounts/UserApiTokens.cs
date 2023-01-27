using System;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Logic.Accounts;

public class UserApiTokens
{
    private string GenerateToken()
    {
        var codes = new GenerateResetCode();
        return codes.GeneratePasswordResetCode(DateTime.Now.ToLongTimeString(), 100);
    }

    public UsersApiTokens Create(int userId)
    {
        using (var cn = InitializeSettings.DbFactory)
        {
            cn.Open();

            var dataMany = cn.Query<UsersApiTokens>("SELECT * FROM UsersApiTokens WHERE UserId=@UserId",
                new { UserId = userId });
            UsersApiTokens data;
            if (dataMany != null && dataMany.Any())
            {
                data = dataMany.First();
                if (data.Token1ExpireTime < DateTime.UtcNow)
                {
                    data.Token1 = GenerateToken();
                    data.Token1ExpireTime = DateTime.UtcNow.AddDays(1);
                }

                if (data.Token2ExpireTime < DateTime.UtcNow)
                {
                    data.Token2 = GenerateToken();
                    data.Token2ExpireTime = DateTime.UtcNow.AddDays(2);
                }

                cn.Update(data);
            }
            else
            {
                data = new UsersApiTokens
                {
                    UserId = userId,
                    Token1 = GenerateToken(),
                    Token1ExpireTime = DateTime.UtcNow.AddDays(1),
                    Token2 = GenerateToken(),
                    Token2ExpireTime = DateTime.UtcNow.AddDays(2)
                };

                cn.Insert(data);
            }

            return data;
        }
    }

    public UsersApiTokens Retrieve(int userid)
    {
        using (var cn = InitializeSettings.DbFactory)
        {
            cn.Open();

            var data = cn.Query<UsersApiTokens>("SELECT * FROM UsersApiTokens WHERE UserId = @UserId",
                new { UserId = userid });
            if (data.Count() > 0)
            {
                // If expired generate a new key
                if (data.FirstOrDefault().Token1ExpireTime <= DateTime.UtcNow) return Create(userid);

                return data.FirstOrDefault();
            }

            return Create(userid);
        }
    }
}