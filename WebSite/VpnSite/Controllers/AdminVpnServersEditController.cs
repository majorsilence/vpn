using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class AdminVpnServersEditController : Controller
    {

        public ActionResult Index(int ?id, string address, int? port, string desc,
            int? region, string active)
        {
            ViewData["id"] = id;
            ViewData["address"] = address;
            ViewData["port"] = port;
            ViewData["desc"] = desc;
            ViewData["region"] = region;
            ViewData["active"] = active;
            return View();
        }

        public ActionResult EditServers(int ? id, string address, int port,
            string desc, int region, string active)
        {
            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return null;
            }


            try
            {

                bool activeYes = false;
                if (active != null)
                {
                    activeYes = true;
                }

                var vpns = new LibLogic.Admin.VpnServers();
                if (id.HasValue)
                {
                    vpns.Update(id.Value, address, 
                        port,
                        desc,
                        region,
                        activeYes);

                }
                else
                {
                    vpns.Insert(address,
                        port,
                        desc,
                        region,
                        activeYes);
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

