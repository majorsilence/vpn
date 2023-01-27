using System;
using System.Collections.Generic;

namespace Majorsilence.Vpn.Site.Models;

public class Users : CustomViewLayout
{
    public Users()
    {
        UserList = Logic.Accounts.UserInfo.RetrieveUserList();
    }

    public IEnumerable<Majorsilence.Vpn.Poco.Users> UserList { get; private set; }
}