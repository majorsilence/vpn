using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Majorsilence.Vpn.Poco
{
    [Dapper.Contrib.Extensions.Table("Users")]
    public class Users
    {

        public Users()
        {
        }

        public Users(string email, string password, string salt, string firstName,
                     string lastName, bool admin, DateTime createTime, string stripeCustomerAccount, 
                     string stripeSubscriptionId,
                     string passwordResetCode, bool isBetaUser)
        {
            this.Email = email;
            this.Password = password;
            this.Salt = salt;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Admin = admin;
            this.CreateTime = createTime;
            this.StripeCustomerAccount = stripeCustomerAccount;
            this.StripeSubscriptionId = stripeSubscriptionId;
            this.PasswordResetCode = passwordResetCode;
            this.IsBetaUser = isBetaUser;
        }

        [Dapper.Contrib.Extensions.Key()]
        public int Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool Admin { get; set; }

        public DateTime CreateTime { get; set; }

        public string StripeCustomerAccount { get; set; }

        public string StripeSubscriptionId { get; set; }

        public string PasswordResetCode { get; set; }

        public bool IsBetaUser { get; set; }
    }
}