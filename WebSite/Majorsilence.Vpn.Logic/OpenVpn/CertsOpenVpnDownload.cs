using System.IO;
using System.Linq;
using Dapper;
using Ionic.Zip;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Logic.OpenVpn;

public class CertsOpenVpnDownload
{
    private readonly DatabaseSettings _dbSettings;

    public CertsOpenVpnDownload(DatabaseSettings dbSettings)
    {
        _dbSettings = dbSettings;
    }

    public byte[] UploadToClient(int userid)
    {
        var ovpn = new InlineOvpnProfile(userid, _dbSettings);

        using (var zip = new ZipFile())
        {
            using (var db = _dbSettings.DbFactory)
            {
                db.Open();

                var userCerts = db.Query<UserOpenVpnCerts>("SELECT * FROM UserOpenVpnCerts WHERE UserId=@UserId",
                    new { UserId = userid });

                if (userCerts.Count() > 0)
                {
                    zip.AddEntry("majorsilencevpn/other/ca.crt", userCerts.First().CertCa);
                    zip.AddEntry("majorsilencevpn/other/majorvpn.key", userCerts.First().CertKey);
                    zip.AddEntry("majorsilencevpn/other/majorvpn.crt", userCerts.First().CertCrt);

                    zip.AddEntry("majorsilencevpn/android/ca.crt", userCerts.First().CertCa);
                    zip.AddEntry("majorsilencevpn/android/majorvpn.key", userCerts.First().CertKey);
                    zip.AddEntry("majorsilencevpn/android/majorvpn.crt", userCerts.First().CertCrt);

                    zip.AddEntry("majorsilencevpn/apple/ca.crt", userCerts.First().CertCa);
                    zip.AddEntry("majorsilencevpn/apple/majorvpn.key", userCerts.First().CertKey);
                    zip.AddEntry("majorsilencevpn/apple/majorvpn.crt", userCerts.First().CertCrt);

                    zip.AddEntry("majorsilencevpn/ubuntu/ca.crt", userCerts.First().CertCa);
                    zip.AddEntry("majorsilencevpn/ubuntu/majorvpn.key", userCerts.First().CertKey);
                    zip.AddEntry("majorsilencevpn/ubuntu/majorvpn.crt", userCerts.First().CertCrt);

                    if (ovpn.OvpnInline.Trim() != "") zip.AddEntry("majorsilencevpn/majorvpn.ovpn", ovpn.OvpnInline);
                    if (ovpn.OvpnNotInline.Trim() != "")
                    {
                        zip.AddEntry("majorsilencevpn/other/MajorVpn-Use-With-Key-And-Crt.ovpn", ovpn.OvpnNotInline);
                        zip.AddEntry("majorsilencevpn/android/majorvpn-android.ovpn", ovpn.OvpnNotInline);
                        zip.AddEntry("majorsilencevpn/apple/majorvpn-iOS.ovpn", ovpn.OvpnNotInline);
                        zip.AddEntry("majorsilencevpn/ubuntu/majorvpn-network-manager.ovpn", ovpn.OvpnNotInline);
                    }
                }
            }


            byte[] ba;
            using (var ms = new MemoryStream())
            {
                zip.Save(ms);
                ba = ms.ToArray();
            }

            return ba;
        }
    }
}