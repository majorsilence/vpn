using System.Collections.Generic;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Accounts;

namespace Majorsilence.Vpn.Site.Models;

public class Users : CustomViewLayout
{
    public Users(DatabaseSettings dbSetings)
    {
        UserList = UserInfo.RetrieveUserList(dbSetings);
    }

    public IEnumerable<Poco.Users> UserList { get; }
}