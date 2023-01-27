using System.Security.Cryptography;
using System.Text;

namespace Majorsilence.Vpn.Logic.Helpers;

public class GenerateResetCode
{
    private static string key = "PFzR6eSE56Tl7WC12ODE2oc8FSlq0O";

    public string GeneratePasswordResetCode(string email, int maxSize)
    {
        var code = "";
        var chars = new char[62];
        key = email + key;
        chars = key.ToCharArray();
        var size = maxSize;
        var data = new byte[1];
        var crypto = new RNGCryptoServiceProvider();
        crypto.GetNonZeroBytes(data);

        data = new byte[size];
        crypto.GetNonZeroBytes(data);
        var result = new StringBuilder(size);
        foreach (var b in data) result.Append(chars[b % (chars.Length - 1)]);
        code = result.ToString();

        return code;
    }

    public string GeneratePasswordResetCode(string email)
    {
        return GeneratePasswordResetCode(email, 12);
    }
}