using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;

namespace Majorsilence.Vpn.Logic.OpenVpn
{
    /// <summary>
    /// Create a ovpn profile for openvpn that inlines the ca, cert, and key
    /// </summary>
    public class InlineOvpnProfile
    {

        Majorsilence.Vpn.Poco.UserOpenVpnCerts userCertData = null;
        Majorsilence.Vpn.Poco.VpnServers serverData = null;

        public InlineOvpnProfile(int userId)
        {

            using (var db = InitializeSettings.DbFactory)
            {
                db.Open();
                var data = db.Query<Majorsilence.Vpn.Poco.UserOpenVpnCerts>("SELECT * FROM UserOpenVpnCerts WHERE UserId=@UserId",
                               new {UserId = userId});
                
                if (data.Count() == 1)
                {
                    userCertData = data.First();
                    serverData = db.Get<Majorsilence.Vpn.Poco.VpnServers>(userCertData.VpnServersId);
                }
                else if (data.Count() > 1)
                {
                    throw new Exceptions.InvalidDataException("InlineOvpnProfile, to many values found.");
                }

               
            }
        }



        public string OvpnNotInline
        {
            get
            {
                if (userCertData == null)
                {
                    return "";  
                }

                var data = new System.Text.StringBuilder();
                data.Append("client");
                data.Append("\r\n");
                data.Append("\r\n");
                data.Append(";See for details: https://openvpn.net/index.php/open-source/documentation/howto.html#client");
                data.Append("\r\n");
                data.Append("\r\n");

                data.Append("dev tun");
                data.Append("\r\n");
                data.Append("proto tcp");
                data.Append("\r\n");
                data.Append(string.Format("remote {0} {1}", serverData.Address, serverData.VpnPort));
                data.Append("\r\n");
                // mitm: see http://openvpn.net/index.php/open-source/documentation/howto.html#mitm
                data.Append("remote-cert-tls server");
                data.Append("\r\n");
                data.Append("\r\n");

                data.Append("resolv-retry infinite");
                data.Append("\r\n");
                data.Append("nobind");
                data.Append("\r\n");
                data.Append("\r\n");

                data.Append("persist-key");
                data.Append("\r\n");
                data.Append("persist-tun");
                data.Append("\r\n");
                data.Append("\r\n");

                data.Append("ca ca.crt");
                data.Append("\r\n");
                data.Append("cert majorvpn.crt");
                data.Append("\r\n");
                data.Append("key majorvpn.key");
                data.Append("\r\n");
                data.Append("\r\n");

                data.Append("verb 1");
                data.Append("\r\n");
                data.Append("\r\n");

                data.Append("keepalive 10 900");
                data.Append("\r\n");
                data.Append("inactive 3600");
                data.Append("\r\n");
                data.Append("comp-lzo");
                data.Append("\r\n");
                data.Append("\r\n");

                return data.ToString();

            }
        }

        public string OvpnInline
        {
            get
            {

                if (userCertData == null)
                {
                    return "";  
                }

                var data = new System.Text.StringBuilder();
                data.Append("client");
                data.Append("\r\n");
                data.Append("\r\n");
                data.Append(";See for details: https://openvpn.net/index.php/open-source/documentation/howto.html#client");
                data.Append("\r\n");
                data.Append("\r\n");

                data.Append("dev tun");
                data.Append("\r\n");
                data.Append("proto tcp");
                data.Append("\r\n");
                data.Append(string.Format("remote {0} {1}", serverData.Address, serverData.VpnPort));
                data.Append("\r\n");
                // mitm: see http://openvpn.net/index.php/open-source/documentation/howto.html#mitm
                data.Append("remote-cert-tls server");
                data.Append("\r\n");
                data.Append("\r\n");

                data.Append("resolv-retry infinite");
                data.Append("\r\n");
                data.Append("nobind");
                data.Append("\r\n");
                data.Append("\r\n");

                data.Append("persist-key");
                data.Append("\r\n");
                data.Append("persist-tun");
                data.Append("\r\n");
                data.Append("\r\n");

                data.Append("ca [inline]");
                data.Append("\r\n");
                data.Append("cert [inline]");
                data.Append("\r\n");
                data.Append("key [inline]");
                data.Append("\r\n");
                data.Append("\r\n");
               
                data.Append("verb 1");
                data.Append("\r\n");
                data.Append("\r\n");

                data.Append("keepalive 10 900");
                data.Append("\r\n");
                data.Append("inactive 3600");
                data.Append("\r\n");
                data.Append("comp-lzo");
                data.Append("\r\n");
                data.Append("\r\n");

                data.Append("<ca>");
                data.Append("\r\n");
                data.Append("\r\n");
                data.Append(System.Text.Encoding.UTF8.GetString(userCertData.CertCa).Replace("\n", "\r\n"));
                data.Append("\r\n");
                data.Append("\r\n");
                data.Append("</ca>");
                data.Append("\r\n");
                data.Append("<cert>");
                data.Append("\r\n");
               
                data.Append("\r\n");
                data.Append(System.Text.Encoding.UTF8.GetString(userCertData.CertCrt).Replace("\n", "\r\n"));
                data.Append("\r\n");
                
                data.Append("\r\n");
                data.Append("</cert>");
                data.Append("\r\n");
                data.Append("<key>");
                data.Append("\r\n");
                
                data.Append("\r\n");
                data.Append(System.Text.Encoding.UTF8.GetString(userCertData.CertKey).Replace("\n", "\r\n"));
                data.Append("\r\n");
                
                data.Append("\r\n");
                data.Append("</key>");
                data.Append("\r\n");


                return data.ToString();

            }
        }

    }
}