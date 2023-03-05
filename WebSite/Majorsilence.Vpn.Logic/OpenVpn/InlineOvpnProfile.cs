using System.Linq;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Exceptions;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Logic.OpenVpn;

/// <summary>
///     Create a ovpn profile for openvpn that inlines the ca, cert, and key
/// </summary>
public class InlineOvpnProfile
{
    private readonly VpnServers serverData;
    private readonly UserOpenVpnCerts userCertData;

    public InlineOvpnProfile(int userId, DatabaseSettings dbSettings)
    {
        using (var db = dbSettings.DbFactory)
        {
            db.Open();
            var data = db.Query<UserOpenVpnCerts>("SELECT * FROM UserOpenVpnCerts WHERE UserId=@UserId",
                new { UserId = userId });

            if (data.Count() == 1)
            {
                userCertData = data.First();
                serverData = db.Get<VpnServers>(userCertData.VpnServersId);
            }
            else if (data.Count() > 1)
            {
                throw new InvalidDataException("InlineOvpnProfile, to many values found.");
            }
        }
    }


    public string OvpnNotInline
    {
        get
        {
            if (userCertData == null) return "";

            var data = new StringBuilder();
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
            if (userCertData == null) return "";

            var data = new StringBuilder();
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
            data.Append(Encoding.UTF8.GetString(userCertData.CertCa).Replace("\n", "\r\n"));
            data.Append("\r\n");
            data.Append("\r\n");
            data.Append("</ca>");
            data.Append("\r\n");
            data.Append("<cert>");
            data.Append("\r\n");

            data.Append("\r\n");
            data.Append(Encoding.UTF8.GetString(userCertData.CertCrt).Replace("\n", "\r\n"));
            data.Append("\r\n");

            data.Append("\r\n");
            data.Append("</cert>");
            data.Append("\r\n");
            data.Append("<key>");
            data.Append("\r\n");

            data.Append("\r\n");
            data.Append(Encoding.UTF8.GetString(userCertData.CertKey).Replace("\n", "\r\n"));
            data.Append("\r\n");

            data.Append("\r\n");
            data.Append("</key>");
            data.Append("\r\n");


            return data.ToString();
        }
    }
}