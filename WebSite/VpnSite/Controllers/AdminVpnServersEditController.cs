using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace VpnSite.Controllers
{
    public class AdminVpnServersEditController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EditServers()
        {
            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return null;
            }

            int id;
            bool isNumeric = int.TryParse(VpnSite.Helpers.GlobalHelper.RequestParam("id").Trim().ToLower(), out id);

            try
            {

                string description = VpnSite.Helpers.GlobalHelper.RequestParam("desc").Trim();
                string address = VpnSite.Helpers.GlobalHelper.RequestParam("address").Trim();
                string activeString = VpnSite.Helpers.GlobalHelper.RequestParam("active");
                bool active = false;
                if (activeString != null)
                {
                    active = true;
                }
                int region = int.Parse(VpnSite.Helpers.GlobalHelper.RequestParam("region"));
                int port = int.Parse(VpnSite.Helpers.GlobalHelper.RequestParam("port"));

                var vpns = new LibLogic.Admin.VpnServers();
                if (isNumeric)
                {
                    vpns.Update(id, address, 
                        port, 
                        description,
                        region,
                        active);

                }
                else
                {
                    vpns.Insert(address,
                        port,
                        description,
                        region,
                        active);
                }


            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
            }

            return View();
        }
    }
}

