﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;

namespace LibLogic.OpenVpn
{
    public class CertsOpenVpnRevokeCommand : ICommand
    {

        private LibPoco.Users userData;
        private Ssh.ISsh sshClient;

        private CertsOpenVpnRevokeCommand()
        {
        }

        public CertsOpenVpnRevokeCommand(int userId, Ssh.ISsh sshClient)
        {
            using (var db = Setup.DbFactory)
            {
                db.Open();
                this.userData = db.Get<LibPoco.Users>(userId); 
            }

            this.sshClient = sshClient;
        }

        public void Execute()
        {

            string certName = "";
            LibPoco.VpnServers vpnData = null;
            using (var db = Setup.DbFactory)
            {
                db.Open();
                var certData = db.Query<LibPoco.UserOpenVpnCerts>("SELECT * FROM UserOpenVpnCerts WHERE UserId=@UserId", 
                                   new {UserId = userData.Id});
                if (certData.Count() == 1)
                {
                    certName = certData.First().CertName;
                    vpnData = db.Get<LibPoco.VpnServers>(certData.First().VpnServersId);
                }
                else if (certData.Count() >= 1)
                {
                    throw new InvalidDataException("Invalid data in the UserOpenVpnCerts for user " + userData.Id);
                }
            }

            if (certName == "")
            {
                return;
            }
          
            ActionLog.Log_BackgroundThread("OpenVpn Revoke Start", this.userData.Id);

            RevokeUserCert(vpnData.Address, certName);

            ActionLog.Log_BackgroundThread("OpenVpn Revoke Finished", this.userData.Id);

        }

        /// <summary>
        /// Revoke a users vpn certificate.  This is generally used when they close their account
        /// or stop making payments.
        /// </summary>
        /// <param name="certName"></param>
        private void RevokeUserCert(string host, string certName)
        {
            // Run Command on ssh server
 
            sshClient.Login(host);

            sshClient.WriteLine("sudo su");
            sshClient.WriteLine("cd /etc/openvpn/easy-rsa/");
            sshClient.WriteLine("source vars");
            sshClient.WriteLine(string.Format("./revoke-full {0}", certName));

            int count = 0;
            string output = "";
            while (output.ToLower().Contains("error 23 at 0 depth lookup:certificate revoked") == false)
            {
                if (count > 20)
                {
                    throw new Exceptions.SshException("Error revoking client cert on vpn server");
                }

                if (output.ToLower().Contains("no such file or directory"))
                {
                    throw new Exceptions.SshException("Error revoking client cert on vpn server");
                }

                if (output.ToLower().Contains("already revoked"))
                {
                    return;
                }

                output += sshClient.Read();
                System.Threading.Thread.Sleep(1000);
                count++;
            }
                
            sshClient.WriteLine("exit");

        }


    }
}