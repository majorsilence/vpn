using System;

namespace Majorsilence.Vpn.Logic.Accounts;

public class CreatePasswords
{
    public CreatePasswords(string password)
    {
        Salt = DateTime.UtcNow.ToString();
        Password = Helpers.Hashes.GetSHA512StringHash(password, Salt);
    }

    public CreatePasswords(string password, string extraSaltDetails)
    {
        Salt = DateTime.UtcNow.ToString() + extraSaltDetails;
        Password = Helpers.Hashes.GetSHA512StringHash(password, Salt);
    }

    public readonly string Password;
    public readonly string Salt;
}