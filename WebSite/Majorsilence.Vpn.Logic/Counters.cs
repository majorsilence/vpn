using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Majorsilence.Vpn.Logic;

public static class Counters
{
    /// <summary>
    /// Return the next number for unique vpn cert names.
    /// </summary>
    /// <param name="txn"></param>
    /// <param name="cn"></param>
    /// <returns></returns>
    public static ulong GetSetVpnNum(IDbTransaction txn, IDbConnection cn)
    {
        var count = cn.Query<Majorsilence.Vpn.Poco.Counters>("SELECT * FROM Counters WHERE Code = @Code",
            new { Code = "VPNCERT" });

        if (count.Count() != 1) throw new Exceptions.InvalidDataException("Invalid counters data for VPNCERT");

        var current = count.First().Num;

        count.First().Num = count.First().Num += 1;
        cn.Update(count.First());

        return current;
    }
}