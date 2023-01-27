namespace VpnSite;

public class PlatformDetection
{
    public enum PlatformType
    {
        Android,
        Ios,
        Ipod,
        Iphone,
        Ipad,
        Ubuntu,
        Windows,
        Mac,
        Linux,
        Other
    }


    /// <summary>
    ///     Get the specified userAgent.
    /// </summary>
    /// <param name="userAgent">context.Request.ServerVariables["HTTP_USER_AGENT"] </param>
    public PlatformType Get(string userAgent)
    {
        userAgent = userAgent.ToLower();

        if (!string.IsNullOrEmpty(userAgent))
        {
            if (userAgent.Contains("android"))
                return PlatformType.Android;
            if (userAgent.Contains("iphone"))
                return PlatformType.Iphone;
            if (userAgent.Contains("ipod"))
                return PlatformType.Ipod;
            if (userAgent.Contains("ipad"))
                return PlatformType.Ipad;
            if (userAgent.Contains("ubuntu"))
                return PlatformType.Ubuntu;
            if (userAgent.Contains("windows nt"))
                return PlatformType.Windows;
            if (userAgent.Contains("mac os x"))
                return PlatformType.Mac;
            if (userAgent.Contains("linux")) return PlatformType.Linux;
        }

        return PlatformType.Other;
    }
}