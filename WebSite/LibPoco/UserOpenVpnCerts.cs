using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibPoco
{
    [Dapper.Contrib.Extensions.Table("UserOpenVpnCerts")]
    public class UserOpenVpnCerts
    {

        public UserOpenVpnCerts() { }
        public UserOpenVpnCerts(int userId, string certName, byte[] certCa, byte[] certCrt, 
            byte[] certKey, bool expired, DateTime createTime, int vpnServersId)
        {
            this.UserId = userId;
            this.CertName = certName;
            this.CertCa = certCa;
            this.CertCrt = certCrt;
            this.CertKey = certKey;
            this.Expired = expired;
            this.CreateTime = createTime;
            this.VpnServersId = vpnServersId;
        }

        [Dapper.Contrib.Extensions.Key()]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string CertName { get; set; }

        public byte[] CertCa { get; set; }

        public byte[] CertCrt { get; set; }

        public byte[] CertKey { get; set; }

        public bool Expired { get; set; }

        public DateTime CreateTime { get; set; }

        public int VpnServersId { get; set; }

    }
}