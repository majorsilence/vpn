using System.Security.Cryptography;
using System.Text;

namespace Majorsilence.Vpn.Logic.Helpers;

public class GenerateResetCode
{
    private readonly IEncryptionKeysSettings _keys;
    public GenerateResetCode(IEncryptionKeysSettings keys)
    {
        _keys = keys;
    }
    
    public string GeneratePasswordResetCode(string email, int maxSize)
    {
        string key = _keys.Key1;
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