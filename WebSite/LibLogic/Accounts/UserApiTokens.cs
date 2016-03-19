using System;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Linq;

namespace LibLogic.Accounts
{
    public class UserApiTokens
    {
        public UserApiTokens()
        {
        }

        private string GenerateToken()
        {

            var codes = new Helpers.GenerateResetCode();
            return codes.GeneratePasswordResetCode(DateTime.Now.ToLongTimeString(), 100);

        }

        public LibPoco.UsersApiTokens Create(int userId)
        {
        
            using (var cn = Setup.DbFactory)
            {
                cn.Open();

                var dataMany = cn.Query<LibPoco.UsersApiTokens>("SELECT * FROM UsersApiTokens WHERE UserId=@UserId",
                                   new {UserId = userId});
                LibPoco.UsersApiTokens data;
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
                    data = new LibPoco.UsersApiTokens()
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

        public LibPoco.UsersApiTokens Retrieve(int userid)
        {
            using (var cn = Setup.DbFactory)
            {
                cn.Open();

                var data = cn.Query<LibPoco.UsersApiTokens>("SELECT * FROM UsersApiTokens WHERE UserId = @UserId",
                               new{UserId = userid});
                if (data.Count() > 0)
                {
                    // If expired generate a new key
                    if (data.FirstOrDefault().Token1ExpireTime <= DateTime.UtcNow)
                    {
                        return Create(userid);
                    }

                    return data.FirstOrDefault();
                }

                return Create(userid);
            }
        }

    }
}

