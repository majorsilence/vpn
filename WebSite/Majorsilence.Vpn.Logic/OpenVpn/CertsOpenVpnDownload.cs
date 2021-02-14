using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Logic.OpenVpn
{
    public class CertsOpenVpnDownload
    {


        public byte[] UploadToClient(int userid)
        {

            var ovpn = new Majorsilence.Vpn.Logic.OpenVpn.InlineOvpnProfile(userid);

            using (ZipFile zip = new ZipFile())
            {
                using (var db = Majorsilence.Vpn.Logic.Setup.DbFactory)
                {
                    db.Open();

                    var userCerts = db.Query<Majorsilence.Vpn.Poco.UserOpenVpnCerts>("SELECT * FROM UserOpenVpnCerts WHERE UserId=@UserId",
                                        new {UserId = userid});

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

                        if (ovpn.OvpnInline.Trim() != "")
                        {
                            zip.AddEntry("majorsilencevpn/majorvpn.ovpn", ovpn.OvpnInline);
                        }
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
                using (var ms = new System.IO.MemoryStream())
                {
                    zip.Save(ms);
                    ba = ms.ToArray();
                }

                return ba;

            }

        }

    }
}
