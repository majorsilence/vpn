using System;

namespace Majorsilence.Vpn.Poco
{
    [Dapper.Contrib.Extensions.Table("BetaKeys")]
    public class BetaKeys
    {
        public BetaKeys()
        {
        }

        public BetaKeys(string code, bool isUsed, bool isSent)
        {
            this.Code = code;
            this.IsUsed = isUsed;
            this.IsSent = isSent;
        }

        [Dapper.Contrib.Extensions.Key()]
        public int Id { get; set; }

        public string Code { get; set; }

        public bool IsUsed { get; set; }

        public bool IsSent{ get; set; }
    }
}

