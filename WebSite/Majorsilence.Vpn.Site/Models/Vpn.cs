using System.Globalization;
using System.Threading;

namespace Majorsilence.Vpn.Site.Models;

public class Vpn
{
    public void InitCultureByPram(string lang)
    {
        var culture = lang;

        if (string.IsNullOrEmpty(culture)) return;

        culture = culture.ToLower().Trim();
        if (culture != "en" && culture != "fr" && culture != "es" && culture != "zh") return;

        if (!string.IsNullOrEmpty(culture))
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
        }
    }

    public void InitCultureByCookie()
    {
        var cultureCookie = "";

        // FIXME: how does this work in asp core?
        //if (System.Web.HttpContext.Current.Request.Cookies["lang"] != null)
        //{
        //    cultureCookie = System.Web.HttpContext.Current.Request.Cookies["lang"].Value;
        //}


        if (string.IsNullOrEmpty(cultureCookie)) return;

        if (cultureCookie != "en" && cultureCookie != "fr" && cultureCookie != "es" && cultureCookie != "zh") return;

        if (!string.IsNullOrEmpty(cultureCookie))
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureCookie);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureCookie);
        }
    }
}