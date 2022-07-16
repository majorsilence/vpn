using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Majorsilence.Vpn.Logic.DTO
{
    [DataContract]
    public class PaypalPaymentResponse
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public DateTime create_time { get; set; }
        [DataMember]
        public DateTime update_time { get; set; }
        [DataMember]
        public string state { get; set; }
        [DataMember]
        public string intent { get; set; }
        [DataMember]
        public PayPalPayer payer { get; set; }
        [DataMember]
        public List<PayPalTransactions> transactions { get; set; }
        [DataMember]
        public List<PayPallinks> links { get; set; }
    }

    [DataContract]
    public class PayPallinks {
        [DataMember]
        public string href { get; set; }
        [DataMember]
        public string rel { get; set; }
        [DataMember]
        public string method { get; set; }
    }
}