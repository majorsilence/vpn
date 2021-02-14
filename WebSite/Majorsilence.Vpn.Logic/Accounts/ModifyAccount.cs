using System;
using Dapper.Contrib.Extensions;

namespace LibLogic.Accounts
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

                var data = db.Get<LibPoco.Users>(userid);

                data.Admin = !data.Admin;

                db.Update(data);

            }
        }

    }
}

