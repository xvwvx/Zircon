// Licensed to X.

using System.Diagnostics;
using Nito.AsyncEx;

namespace Mir3.Shared.Utils;

[DebuggerDisplay("UpdateTick: {Delta} {TickCount}")]
public readonly struct UpdateTick(uint delta, uint tickCount)
{
    public readonly uint Delta = delta;
    public readonly uint TickCount = tickCount;

    public long Time => (long)TickCount * Delta;

    public override string ToString()
    {
        return $"UpdateTick: {Time} {TickCount}";
    }
}

public static class Runner
{
    // 绑定线程的AsyncLoop
    public static ValueTask RunFixedTickLoopOnThreadAsync(uint ticksPerSecond, Func<UpdateTick, ValueTask> fixedUpdate,
        CancellationToken ct)
    {
        AsyncContext.Run(async () => { await RunFixedTickLoopAsync(ticksPerSecond, fixedUpdate, ct).ConfigureAwait(false); });
        return ValueTask.CompletedTask;
    }

    public static async ValueTask RunFixedTickLoopAsync(
        uint ticksPerSecond,
        Func<UpdateTick, ValueTask> fixedUpdate,
        CancellationToken ct
    )
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var delta = 1000 / ticksPerSecond;
            var lastMilliseconds = stopwatch.ElapsedMilliseconds;

            uint tickCount = 0;
            uint accumulation = 0;
            while (!ct.IsCancellationRequested)
            {
                var currentMilliseconds = stopwatch.ElapsedMilliseconds;
                accumulation += (uint)(currentMilliseconds - lastMilliseconds);
                lastMilliseconds = currentMilliseconds;

                if (delta <= accumulation)
                {
                    accumulation -= delta;
                    tickCount++;
                    await fixedUpdate(new UpdateTick(delta, tickCount)).ConfigureAwait(false);
                }
                else
                {
                    if (accumulation <= 5)
                    {
                        await Task.Delay(1, ct).ConfigureAwait(false);
                    }
                    else
                    {
                        await Task.Yield();
                    }
                }
            }
        }
        finally
        {
            stopwatch.Stop();
        }
    }
}
