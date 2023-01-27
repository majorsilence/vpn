using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Majorsilence.Vpn.Logic.Helpers;

public class Hashes
{
    /// <summary>
    /// Creates a SHA512 hash of a string.
    /// </summary>
    /// <param name="nonHashedString">
    /// The string to hash<see cref="System.String"/>
    /// </param>
    /// <param name="salt"></param>
    /// <returns>
    /// A SHA512 hash <see cref="System.String"/>
    /// </returns>
    /// 
    public static string GetSHA512StringHash(string nonHashedString, string salt)
    {
        var hashValue = "";
        using (SHA512 _sha512 = new SHA512Managed())
        {
            var data = System.Text.Encoding.UTF8.GetBytes(salt + nonHashedString);
            var retVal = _sha512.ComputeHash(data);
            // hex string
            hashValue = BitConverter.ToString(retVal).Replace("-", "");
        }


        return hashValue;
    }
}