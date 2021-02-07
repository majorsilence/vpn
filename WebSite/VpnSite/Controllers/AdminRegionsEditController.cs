using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class AdminRegionsEditController : Controller
    {

        public ActionResult Index(int ?id, string desc, string active)
        {
            ViewData["id"] = id;
            ViewData["desc"] = desc;
            ViewData["active"] = active;
            return View();
        }

        public ActionResult EditRegions(int ?id, string desc, string active)
        {
            if (Helpers.SessionVariables.Instance.LoggedIn == false || Helpers.SessionVariables.Instance.IsAdmin == false)
            {
                return null;
            }

            int n;

            var edit = new LibLogic.Admin.Regions();


            bool activeYes = false;
            if (active != null)
            {
                activeYes = true;
            }

            if (id.HasValue)
            {
                edit.Update(id.Value, desc, activeYes);
            }
            else
            {
                edit.Insert(desc, activeYes);
            }


            return View();
        }
    }
}

