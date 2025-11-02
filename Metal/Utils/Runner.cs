// Licensed to X.

using System.Diagnostics;

namespace Metal.Utils;

[DebuggerDisplay("UpdateTick: {Delta} {TickCount} {Time}")]
public readonly struct UpdateTick(uint delta, uint tickCount)
{
    public readonly uint Delta = delta;
    public readonly uint TickCount = tickCount;

    public long Time => (long)TickCount * Delta;
}

public static class Runner
{
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
