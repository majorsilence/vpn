using System.Data;
using System.Linq;
using System.Timers;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Helpers;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Majorsilence.Vpn.Logic;

public class DatabaseSettings
{
    public string StrConnectionVpn { get; }
    public string StrConnectionSessions { get; }
    public bool IsLiveSite { get; }
    private readonly ILogger _logger;
    public IEmail Email { get; private set; }
    public IDbConnection DbFactory => new MySqlConnection(StrConnectionVpn);

    public IDbConnection DbFactoryWithoutDatabase =>
        // return new MySql.Data.MySqlClient.MySqlConnection(string.Format("Server={0};Uid={1};Pwd={2};Port={3};CharSet=utf8mb4;",
        //   server, username, password, port));
        new MySqlConnection(StrConnectionVpn);


    public DatabaseSettings(string strVpnConnection, string sessionsConnection, IEmail email, bool isLiveSite,
        ILogger logger)
    {
        StrConnectionVpn = strVpnConnection;
        StrConnectionSessions = sessionsConnection;

        Email = email;
        this.IsLiveSite = isLiveSite;
        _logger = logger;
    }
}