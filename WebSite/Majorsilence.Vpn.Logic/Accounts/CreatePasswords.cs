using System;
using Majorsilence.Vpn.Logic.Helpers;

namespace Majorsilence.Vpn.Logic.Accounts;

public class CreatePasswords
{
    public readonly string Password;
    public readonly string Salt;

    public CreatePasswords(string password)
    {
        Salt = DateTime.UtcNow.ToString();
        Password = Hashes.GetSHA512StringHash(password, Salt);
    }

    public CreatePasswords(string password, string extraSaltDetails)
    {
        Salt = DateTime.UtcNow + extraSaltDetails;
        Password = Hashes.GetSHA512StringHash(password, Salt);
    }
}