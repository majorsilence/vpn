using System;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Poco;

[Table("Users")]
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
        Email = email;
        Password = password;
        Salt = salt;
        FirstName = firstName;
        LastName = lastName;
        Admin = admin;
        CreateTime = createTime;
        StripeCustomerAccount = stripeCustomerAccount;
        StripeSubscriptionId = stripeSubscriptionId;
        PasswordResetCode = passwordResetCode;
        IsBetaUser = isBetaUser;
    }

    [Key] public int Id { get; set; }

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