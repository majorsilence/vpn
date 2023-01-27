using System.Web;

namespace Majorsilence.Vpn.Site.Helpers;

public class GlobalHelper
{
    public static string UriEncode(string value)
    {
        return HttpUtility.UrlEncode(value);
    }

    public static string UriDecode(string value)
    {
        return HttpUtility.UrlDecode(value);
    }

    public static string HtmlEncode(string value)
    {
        return HttpUtility.HtmlEncode(value);
    }
}