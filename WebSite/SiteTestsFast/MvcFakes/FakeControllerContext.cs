using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteTestsFast.MvcFakes
{
    static class FakeControllerContext 
    {
        public static void SetContext(ControllerBase controller, NameValueCollection headers)
        {
            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor()
            };

            controller.ControllerContext = new ControllerContext(actionContext);
            if (headers != null)
            {
                foreach (var item in headers.AllKeys)
                {
                    controller.Request.Headers.Add(item, headers.Get(item));
                }
            }
        }
    }
}
