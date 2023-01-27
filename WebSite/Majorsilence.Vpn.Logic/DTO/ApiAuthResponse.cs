using System;

namespace Majorsilence.Vpn.Logic.DTO;

public class ApiAuthResponse
{
    public ApiAuthResponse()
    {
    }

    public string Token1 { get; set; }

    public DateTime Token1ExpireUtc { get; set; }

    public string Token2 { get; set; }

    public DateTime Token2ExpireUtc { get; set; }

    public int UserId { get; set; }
}