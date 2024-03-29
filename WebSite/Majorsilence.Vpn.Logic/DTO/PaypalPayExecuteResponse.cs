﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Majorsilence.Vpn.Logic.DTO;

/// <summary>
///     Can add payers details and refund details if necessary
/// </summary>
[DataContract]
public class PaypalPayExecuteResponse
{
    [DataMember] public string id { get; set; }
    [DataMember] public DateTime create_time { get; set; }
    [DataMember] public DateTime update_time { get; set; }
    [DataMember] public string state { get; set; }
    [DataMember] public string intent { get; set; }
    [DataMember] public PayPalPayer payer { get; set; }
    [DataMember] public List<PayPalTransactions> transactions { get; set; }
}