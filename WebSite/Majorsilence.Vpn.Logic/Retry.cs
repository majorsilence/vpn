using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Majorsilence.Vpn.Logic;

// https://stackoverflow.com/questions/1563191/cleanest-way-to-write-retry-logic
// See https://github.com/App-vNext/Polly for a more advanced option
public static class Retry
{
    public static void Do(
        Action action,
        TimeSpan retryInterval,
        int maxAttemptCount = 3)
    {
        Do<object>(() =>
        {
            action();
            return null;
        }, retryInterval, maxAttemptCount);
    }

    public static async Task DoAsync(
        Action action,
        TimeSpan retryInterval,
        int maxAttemptCount = 3)
    {
        await DoAsync<object>(() =>
        {
            action();
            return null;
        }, retryInterval, maxAttemptCount);
    }

    public static T Do<T>(
        Func<T> action,
        TimeSpan retryInterval,
        int maxAttemptCount = 3)
    {
        var exceptions = new List<Exception>();

        for (var attempted = 0; attempted < maxAttemptCount; attempted++)
            try
            {
                if (attempted > 0) Thread.Sleep(retryInterval);
                return action();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

        throw new AggregateException(exceptions);
    }

    public static async Task<T> DoAsync<T>(
        Func<T> action,
        TimeSpan retryInterval,
        int maxAttemptCount = 3)
    {
        var exceptions = new List<Exception>();

        for (var attempted = 0; attempted < maxAttemptCount; attempted++)
            try
            {
                if (attempted > 0) await Task.Delay(retryInterval);
                return action();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

        throw new AggregateException(exceptions);
    }
}