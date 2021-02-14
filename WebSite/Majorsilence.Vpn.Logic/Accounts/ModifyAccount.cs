using System;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Logic.Accounts
{
    public class ModifyAccount
    {
        public ModifyAccount()
        {
        }

        public void ToggleIsAdmin(int userid)
        {
            using (var db = Setup.DbFactory)
            {
                db.Open();

                var data = db.Get<Majorsilence.Vpn.Poco.Users>(userid);

                data.Admin = !data.Admin;

                db.Update(data);

            }
        }

    }
}

