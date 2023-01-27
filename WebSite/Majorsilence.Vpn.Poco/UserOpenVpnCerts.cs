﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Majorsilence.Vpn.Poco;

[Dapper.Contrib.Extensions.Table("UserOpenVpnCerts")]
public class UserOpenVpnCerts
{
    public UserOpenVpnCerts()
    {
    }

    public UserOpenVpnCerts(int userId, string certName, byte[] certCa, byte[] certCrt,
        byte[] certKey, bool expired, DateTime createTime, int vpnServersId)
    {
        UserId = userId;
        CertName = certName;
        CertCa = certCa;
        CertCrt = certCrt;
        CertKey = certKey;
        Expired = expired;
        CreateTime = createTime;
        VpnServersId = vpnServersId;
    }

    [Dapper.Contrib.Extensions.Key()] public int Id { get; set; }

    public int UserId { get; set; }

    public string CertName { get; set; }

    public byte[] CertCa { get; set; }

    public byte[] CertCrt { get; set; }

    public byte[] CertKey { get; set; }

    public bool Expired { get; set; }

    public DateTime CreateTime { get; set; }

    public int VpnServersId { get; set; }
}