using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LibLogic.Helpers
{
    public class GenerateResetCode
    {
        private static string key = "PFzR6eSE56Tl7WC12ODE2oc8FSlq0O";

        public string GeneratePasswordResetCode(string email, int maxSize)
        {
            string code = "";
            char[] chars = new char[62];
            key = email + key;
            chars = key.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);

            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {

                result.Append(chars[b % (chars.Length - 1)]);
            }
            code = result.ToString();

            return code;
        }

        public string GeneratePasswordResetCode(string email)
        {
            return GeneratePasswordResetCode(email, 12);
        }

    }
}
