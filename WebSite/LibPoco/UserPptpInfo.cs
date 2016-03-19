using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibPoco
{
    [Dapper.Contrib.Extensions.Table("UserPptpInfo")]
    public class UserPptpInfo
    {
        public UserPptpInfo()
        {
        }

        public UserPptpInfo(int userId, bool expired, DateTime createTime, int vpnServersId, string password)
        {
            this.UserId = userId;
            this.Expired = expired;
            this.CreateTime = createTime;
            this.VpnServersId = vpnServersId;
            this.Password = password;
        }

        [Dapper.Contrib.Extensions.Key()]
        public int Id { get; set; }

        public int UserId { get; set; }

        public bool Expired { get; set; }

        public DateTime CreateTime { get; set; }

        public int VpnServersId { get; set; }

        public string Password { get; set; }
    }
}
