using System;

namespace LibPoco
{
    [Dapper.Contrib.Extensions.Table("UsersApiTokens")]
    public class UsersApiTokens
    {
        public UsersApiTokens()
        {
        }

        [Dapper.Contrib.Extensions.Key()]
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to users table
        /// </summary>
        /// <value>The user identifier.</value>
        public int UserId { get; set; }

        public string Token1 { get; set; }

        public DateTime Token1ExpireTime { get; set; }

        public string Token2 { get; set; }

        public DateTime Token2ExpireTime { get; set; }
    }
}

