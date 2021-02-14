using System;

namespace Majorsilence.Vpn.Poco
{
    [Dapper.Contrib.Extensions.Table("ActionLog")]
    public class ActionLog
    {
        public ActionLog()
        {
        }

        [Dapper.Contrib.Extensions.Key()]
        public ulong Id { get; set; }

        public string Action { get; set; }

        public int UserId { get; set; }

        public DateTime ActionDate { get; set; }

    }
}

