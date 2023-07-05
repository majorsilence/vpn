using System.Data.Common;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Majorsilence.Vpn.Logic;

public class DatabaseSettings
{
    private readonly ILogger _logger;


    public DatabaseSettings(string strVpnConnection, string sessionsConnection, bool isLiveSite,
        ILogger logger)
    {
        StrConnectionVpn = strVpnConnection;
        StrConnectionSessions = sessionsConnection;
        IsLiveSite = isLiveSite;
        _logger = logger;
    }

    public string StrConnectionVpn { get; }
    public string StrConnectionSessions { get; }
    public bool IsLiveSite { get; }
    public DbConnection DbFactory => new MySqlConnection(StrConnectionVpn);

    public DbConnection DbFactoryWithoutDatabase =>
        // return new MySql.Data.MySqlClient.MySqlConnection(string.Format("Server={0};Uid={1};Pwd={2};Port={3};CharSet=utf8mb4;",
        //   server, username, password, port));
        new MySqlConnection(StrConnectionVpn);
}