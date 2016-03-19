using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace LibLogic.Helpers
{
    public static class SslSecurity
    {

        public static void Callback()
        {
        
            ServicePointManager.ServerCertificateValidationCallback = (System.Net.Security.RemoteCertificateValidationCallback)
                Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback,
                new System.Net.Security.RemoteCertificateValidationCallback(SslSecurity.Validator));

        }

        public static bool Validator(object sender, System.Security.Cryptography.X509Certificates.X509Certificate cert, 
                                     System.Security.Cryptography.X509Certificates.X509Chain chain, 
                                     System.Net.Security.SslPolicyErrors error)
        {
            return true;
        }


    }
}

