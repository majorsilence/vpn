using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VpnSite.Helpers
{
    public class GlobalHelper
    {
        public static string RequestParam(string paramName)
        {
            string Result = String.Empty;

            if (HttpContext.Current.Request.Form.Count != 0)
            {
                Result = Convert.ToString(HttpContext.Current.Request.Form[paramName]);

                if (Result == null)
                {
                    // for whatever reason the page has both form and QueryString content 
                    // and the value we are looking for is in the query string
                    if (HttpContext.Current.Request.QueryString.Count != 0)
                    {
                        Result = Convert.ToString(HttpContext.Current.Request.QueryString[paramName]);
                    }
                }

            }
            else if (HttpContext.Current.Request.QueryString.Count != 0)
            {
                Result = Convert.ToString(HttpContext.Current.Request.QueryString[paramName]);
            }

            if (Result != null)
            {
                return Result.Trim();
            }

            return Result;
        }

        /// <summary>
        /// Return Encoded parameters Decoded
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static string RequestEncodedParam(string paramName)
        {
            string Result = null;

            if (HttpContext.Current.Request.Form.Count != 0)
            {
                Result = UriDecode(Convert.ToString(HttpContext.Current.Request.Form[paramName]));

                if (Result == null)
                {
                    // for whatever reason the page has both form and QueryString content 
                    // and the value we are looking for is in the query string
                    if (HttpContext.Current.Request.QueryString.Count != 0)
                    {
                        Result = UriDecode(Convert.ToString(HttpContext.Current.Request.QueryString[paramName]));
                    }
                }

            }
            else if (HttpContext.Current.Request.QueryString.Count != 0)
            {
                Result = UriDecode(Convert.ToString(HttpContext.Current.Request.QueryString[paramName]));
            }


            if (Result != null)
            {
                return Result.Trim();
            }

            return Result;

        }

        public static string UriEncode(string value)
        {
            string encodevalue = HttpUtility.UrlEncode(value);
            return encodevalue;
        }

        public static string UriDecode(string value)
        {
            string decodevalue = HttpUtility.UrlDecode(value);
            return decodevalue;
        }
    }
}