using System;
using System.Web;

namespace VpnSite
{
    public class PlatformDetection
    {
        public PlatformDetection()
        {
        }


        /// <summary>
        /// Get the specified userAgent.
        /// </summary>
        /// <param name="userAgent">context.Request.ServerVariables["HTTP_USER_AGENT"] </param>
        public PlatformType Get(string userAgent)
        {
            userAgent = userAgent.ToLower();

            if (!string.IsNullOrEmpty(userAgent))
            {

                if (userAgent.Contains("android"))
                {
                    return PlatformType.Android;
                }
                else if (userAgent.Contains("iphone"))
                {
                    return PlatformType.Iphone;
                }
                else if (userAgent.Contains("ipod"))
                {
                    return PlatformType.Ipod;
                }
                else if (userAgent.Contains("ipad"))
                {
                    return PlatformType.Ipad;
                }
                else if (userAgent.Contains("ubuntu"))
                {
                    return PlatformType.Ubuntu;
                }
                else if (userAgent.Contains("windows nt"))
                {
                    return PlatformType.Windows;
                }
                else if (userAgent.Contains("mac os x"))
                {
                    return PlatformType.Mac;
                }
                else if (userAgent.Contains("linux"))
                {
                    return PlatformType.Linux;
                }
               
            }

            return PlatformType.Other;

        }

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

    }
}

