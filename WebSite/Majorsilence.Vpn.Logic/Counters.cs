using System.Data;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Majorsilence.Vpn.Logic.Exceptions;

namespace Majorsilence.Vpn.Logic;

public static class Counters
{
    /// <summary>
    ///     Return the next number for unique vpn cert names.
    /// </summary>
    /// <param name="txn"></param>
    /// <param name="cn"></param>
    /// <returns></returns>
    public static ulong GetSetVpnNum(IDbTransaction txn, IDbConnection cn)
    {
        var count = cn.Query<Poco.Counters>("SELECT * FROM Counters WHERE Code = @Code",
            new { Code = "VPNCERT" });

        if (count.Count() != 1) throw new InvalidDataException("Invalid counters data for VPNCERT");

        var current = count.First().Num;

        count.First().Num = count.First().Num += 1;
        cn.Update(count.First());

        return current;
    }
}