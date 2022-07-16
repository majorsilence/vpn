using System;

namespace Majorsilence.Vpn.Logic.Accounts
{
    public class CreatePasswords
    {
        public CreatePasswords(string password)
        {
            this.Salt = DateTime.UtcNow.ToString();
            this.Password = Helpers.Hashes.GetSHA512StringHash(password, this.Salt);
        }
        public CreatePasswords(string password, string extraSaltDetails)
        {
            this.Salt = DateTime.UtcNow.ToString() + extraSaltDetails;
            this.Password = Helpers.Hashes.GetSHA512StringHash(password, this.Salt);
        }

        public readonly string Password;
        public readonly string Salt;

    }
}

