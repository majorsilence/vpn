using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Poco;

[Table("VpnServers")]
public class VpnServers
{
    public VpnServers()
    {
    }

    public VpnServers(string address, int vpnPort, string description, int regionId, bool active)
    {
        Address = address;
        VpnPort = vpnPort;
        Description = description;
        RegionId = regionId;
        Active = active;
    }

    [Key] public int Id { get; set; }

    public string Address { get; set; }

    public int VpnPort { get; set; }

    public string Description { get; set; }

    public int RegionId { get; set; }

    public bool Active { get; set; }
}