﻿using System;

namespace LibPoco
{

    [Dapper.Contrib.Extensions.Table("Privacy")]
    public class Privacy
    {

        [Dapper.Contrib.Extensions.Key()]
        public int Id { get; set; }

        public string Policy { get; set; }

        public DateTime CreateTime { get; set; }

    }

}

