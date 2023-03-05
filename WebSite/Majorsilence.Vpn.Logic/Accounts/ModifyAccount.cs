using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Poco;

namespace Majorsilence.Vpn.Logic.Accounts;

public class ModifyAccount
{
    private readonly DatabaseSettings _dbSettings;
    public ModifyAccount(DatabaseSettings dbSettings)
    {
        _dbSettings = dbSettings;
    }
    
    public void ToggleIsAdmin(int userid)
    {
        using (var db = _dbSettings.DbFactory)
        {
            db.Open();

            var data = db.Get<Users>(userid);

            data.Admin = !data.Admin;

            db.Update(data);
        }
    }
}