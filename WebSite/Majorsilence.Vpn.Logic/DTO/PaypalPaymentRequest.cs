using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Majorsilence.Vpn.Logic.DTO
{
    public class PaypalPaymentRequest
    {
        public string intent { get; set; }
        public PayPalRedirect_urls redirect_urls { get; set; }
        public PayPalPayer payer { get; set; }
        public List<PayPalTransactions> transactions { get; set; }
    }

    [DataContract]
    public class PayPalPayer
    {
        [DataMember]
        public string payment_method { get; set; }
    }

    [DataContract]
    public class PayPalAmount
    {
        [DataMember]
        public string total { get; set; }
        [DataMember]
        public string currency { get; set; }
    }

    public class PayPalRedirect_urls
    {

        public string return_url { get; set; }

        public string cancel_url { get; set; }

    }
    [DataContract]
    public class PayPalTransactions
    {
        [DataMember]
        public PayPalAmount amount { get; set; }
        [DataMember]
        public string description { get; set; }
    }
}