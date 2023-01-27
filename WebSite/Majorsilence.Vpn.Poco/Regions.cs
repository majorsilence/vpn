using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Majorsilence.Vpn.Poco;

[Dapper.Contrib.Extensions.Table("Regions")]
public class Regions
{
    public Regions()
    {
    }

    public Regions(string description, bool active)
    {
        Description = description;
        Active = active;
    }

    [Dapper.Contrib.Extensions.Key()] public int Id { get; set; }

    public string Description { get; set; }

    public bool Active { get; set; }
}