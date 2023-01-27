using System.Collections.Generic;
using Majorsilence.Vpn.Logic.Accounts;

namespace Majorsilence.Vpn.Site.Models;

public class Users : CustomViewLayout
{
    public Users()
    {
        UserList = UserInfo.RetrieveUserList();
    }

    public IEnumerable<Poco.Users> UserList { get; }
}