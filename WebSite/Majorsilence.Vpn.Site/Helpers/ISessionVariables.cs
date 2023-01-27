using System;

namespace Majorsilence.Vpn.Site.Helpers;

public interface ISessionVariables
{
    bool LoggedIn { get; set; }


    string Username { get; set; }

    int UserId { get; set; }

    /// <summary>
    /// Check if the logged in user is an admin
    /// </summary>
    bool IsAdmin { get; set; }
}