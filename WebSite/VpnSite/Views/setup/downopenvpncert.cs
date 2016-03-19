using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Web.Routing;

namespace VpnSite.views.setup
{
    /// <summary>
    /// Summary description for downloadcert
    /// </summary>
    public class downopenvpncert : Helpers.IHttpHandlerBase, IRequiresSessionState
    {

        public RequestContext RequestContext { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.Buffer = true;
            context.Response.ContentType = "application/zip; name=Certs.zip";
            context.Response.AddHeader("Content-Disposition", "filename=Certs.zip");

            WriteZippedCertsToResponseStream(ref context, Helpers.SessionVariables.Instance.UserId);
        }

        private void WriteZippedCertsToResponseStream(ref HttpContext context, int userid)
        {
            var dl = new LibLogic.OpenVpn.CertsOpenVpnDownload();
            var str = new System.IO.MemoryStream();
            var fileBytes = dl.UploadToClient(userid);

            context.Response.OutputStream.Write(fileBytes, 0, fileBytes.Length);
            context.Response.Flush();
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