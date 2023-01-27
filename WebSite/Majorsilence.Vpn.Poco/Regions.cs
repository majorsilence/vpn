using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Poco;

[Table("Regions")]
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

    [Key] public int Id { get; set; }

    public string Description { get; set; }

    public bool Active { get; set; }
}