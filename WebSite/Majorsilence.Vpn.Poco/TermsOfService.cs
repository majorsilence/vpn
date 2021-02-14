﻿using System;

namespace LibPoco
{
    [Dapper.Contrib.Extensions.Table("TermsOfService")]
    public class TermsOfService
    {

        [Dapper.Contrib.Extensions.Key()]
        public int Id { get; set; }

        public string Terms { get; set; }

        public DateTime CreateTime { get; set; }

    }
}

