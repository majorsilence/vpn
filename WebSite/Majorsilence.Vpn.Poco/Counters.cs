﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibPoco
{
    [Dapper.Contrib.Extensions.Table("Counters")]
    public class Counters
    {
        public Counters() { }
        public Counters(string code, string description, ulong num)
        {
            this.Code = code;
            this.Description = description;
            this.Num = num;
        }

        [Dapper.Contrib.Extensions.Key()]
        public int Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public ulong Num { get; set; }

    }
}