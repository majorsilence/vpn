using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Logic.Accounts;

public class ModifyAccount
{
    public void ToggleIsAdmin(int userid)
    {
        using (var db = InitializeSettings.DbFactory)
        {
            db.Open();

            var data = db.Get<Users>(userid);

            data.Admin = !data.Admin;

            db.Update(data);
        }
    }
}