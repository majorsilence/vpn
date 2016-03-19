using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.Routing;

namespace VpnSite.views.setup
{
    /// <summary>
    /// Summary description for saveuservpnserver
    /// </summary>
    public class saveuservpnserver : Helpers.IHttpHandlerBase, IRequiresSessionState
    {
        public RequestContext RequestContext { get; set; }

        public void ProcessRequest(HttpContext context)
        {

            if (Helpers.SessionVariables.Instance.LoggedIn == false)
            {
                return;
            }

            int vpnServerId = Convert.ToInt32(Helpers.GlobalHelper.RequestParam("VpnId"));

            context.Response.ContentType = "text/json";
            try
            {
                VpnServer(vpnServerId);            
            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);

                context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                context.Response.Write(ex.Message);
                context.Response.StatusDescription = ex.Message;
                return;
            }


            try
            {
                PptpServer(vpnServerId);
            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);


                context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                context.Response.Write(ex.Message);
                context.Response.StatusDescription = ex.Message;

                using (var ssh = new LibLogic.Ssh.LiveSsh(LibLogic.Helpers.SiteInfo.SshPort, 
                                     LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
                {
                    var revokeOVPN = new LibLogic.OpenVpn.CertsOpenVpnRevokeCommand(Helpers.SessionVariables.Instance.UserId, ssh);
                    revokeOVPN.Execute();
                }

                return;
            }

            var model = new Models.Setup();

            context.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;

            var output = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            context.Response.Write(output);

        }

        private void VpnServer(int vpnServerId)
        {

            using (var sshNewServer = new LibLogic.Ssh.LiveSsh(LibLogic.Helpers.SiteInfo.SshPort, 
                                          LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
            using (var sshRevokeServer = new LibLogic.Ssh.LiveSsh(LibLogic.Helpers.SiteInfo.SshPort, 
                                             LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
            using (var sftp = new LibLogic.Ssh.LiveSftp(LibLogic.Helpers.SiteInfo.SshPort, 
                                  LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
            {
                var cert = new LibLogic.OpenVpn.CertsOpenVpnGenerateCommand(Helpers.SessionVariables.Instance.UserId, 
                               vpnServerId, sshNewServer, sshRevokeServer, sftp);
                cert.Execute();
            }

        }

        private void PptpServer(int vpnServerId)
        {
            using (var sshNewServer = new LibLogic.Ssh.LiveSsh(LibLogic.Helpers.SiteInfo.SshPort, 
                                          LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
            using (var sshRevokeServer = new LibLogic.Ssh.LiveSsh(LibLogic.Helpers.SiteInfo.SshPort, 
                                             LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
            {
                var pptp = new LibLogic.Ppp.ManagePPTP(Helpers.SessionVariables.Instance.UserId, vpnServerId, 
                               sshNewServer, sshRevokeServer);
                pptp.AddUser();
            }
        }

        private void IpsecServer(int vpnServerId)
        {
            using (var sshNewServer = new LibLogic.Ssh.LiveSsh(LibLogic.Helpers.SiteInfo.SshPort, 
                                          LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
            using (var sshRevokeServer = new LibLogic.Ssh.LiveSsh(LibLogic.Helpers.SiteInfo.SshPort, 
                                             LibLogic.Helpers.SiteInfo.VpnSshUser, LibLogic.Helpers.SiteInfo.VpnSshPassword))
            {
                var ipsec = new LibLogic.Ppp.IpSec(Helpers.SessionVariables.Instance.UserId, vpnServerId, sshNewServer,
                                sshRevokeServer);
                ipsec.AddUser();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}