using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Majorsilence.Vpn.Logic.Helpers;

public static class SslSecurity
{
    public static void Callback()
    {
        ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)
            Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback,
                new RemoteCertificateValidationCallback(Validator));
    }

    public static bool Validator(object sender, X509Certificate cert,
        X509Chain chain,
        SslPolicyErrors error)
    {
        return true;
    }
}