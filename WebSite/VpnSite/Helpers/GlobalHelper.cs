using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Majorsilence.Vpn.Site.Helpers
{
    public class GlobalHelper
    {
        public static string RequestParam(string paramName, HttpContext context)
        {
            string Result = String.Empty;

            if (context.Request.Form.Count != 0)
            {
                Result = Convert.ToString(context.Request.Form[paramName]);

                if (Result == null)
                {
                    // for whatever reason the page has both form and QueryString content 
                    // and the value we are looking for is in the query string
                    if (context.Request.QueryString.Count != 0)
                    {
                        Result = Convert.ToString(context.Request.QueryString[paramName]);
                    }
                }

            }
            else if (context.Request.QueryString.Count != 0)
            {
                Result = Convert.ToString(context.Request.QueryString[paramName]);
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
        public static string RequestEncodedParam(string paramName, HttpContext context)
        {
            string Result = null;

            if (context.Request.Form.Count != 0)
            {
                Result = UriDecode(Convert.ToString(context.Request.Form[paramName]));

                if (Result == null)
                {
                    // for whatever reason the page has both form and QueryString content 
                    // and the value we are looking for is in the query string
                    if (context.Request.QueryString.Count != 0)
                    {
                        Result = UriDecode(Convert.ToString(context.Request.QueryString[paramName]));
                    }
                }

            }
            else if (context.Request.QueryString.Count != 0)
            {
                Result = UriDecode(Convert.ToString(context.Request.QueryString[paramName]));
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