using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Vpn.Site.Controllers
{
    public class SetupController : Controller
    {
        readonly ISessionVariables sessionInstance;
        public SetupController(ISessionVariables sessionInstance)
        {
            this.sessionInstance = sessionInstance;
        }

        public ActionResult Index()
        {
            var model = new Models.Setup(sessionInstance.UserId, sessionInstance.Username)
            {
                SessionVariables = sessionInstance
            };
            return View(model);
        }
    }
}
