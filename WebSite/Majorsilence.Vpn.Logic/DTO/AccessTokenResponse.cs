using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Majorsilence.Vpn.Logic.DTO;

/// <summary>
/// 
/// </summary>
[DataContract]
public class AccessTokenResponse
{
    /// <summary>
    /// 
    /// </summary>
    [DataMember]
    public string scope { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DataMember]
    public string access_token { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DataMember]
    public string token_type { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DataMember]
    public string app_id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DataMember]
    public long expires_in { get; set; }
}