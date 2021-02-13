using System;
using System.Collections.Generic;

namespace Majorsilence.Vpn.Site.Models
{
    public class Users : CustomViewLayout
    {
        public Users()
        {
           
            UserList = LibLogic.Accounts.UserInfo.RetrieveUserList();

        }

        public IEnumerable<LibPoco.Users> UserList { get; private set; }
    }
}

