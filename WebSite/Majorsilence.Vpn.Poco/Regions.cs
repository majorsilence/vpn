﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibPoco
{
    [Dapper.Contrib.Extensions.Table("Regions")]
    public class Regions
    {

        public Regions() { }
        public Regions(string description, bool active)
        {
            this.Description = description;
            this.Active = active;
        }

        [Dapper.Contrib.Extensions.Key()]
        public int Id { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }

    }
}