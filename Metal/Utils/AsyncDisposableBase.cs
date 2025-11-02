// Licensed to X.

namespace Metal.Utils;

public abstract class AsyncDisposableBase : IAsyncDisposable
{
    private bool _disposed;

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    protected async ValueTask DisposeAsync(bool disposing)
    {
        if (Interlocked.Exchange(ref _disposed, true))
        {
            return;
        }

        if (disposing)
        {
            await DisposeManagedAsync().ConfigureAwait(false);
        }

        DisposeUnmanaged();
    }

    // 同步释放托管资源
    protected virtual void DisposeManaged()
    {
    }

    // 异步释放托管资源
    protected virtual ValueTask DisposeManagedAsync()
    {
        return ValueTask.CompletedTask;
    }

    // 释放非托管资源
    protected virtual void DisposeUnmanaged()
    {
    }

    ~AsyncDisposableBase()
    {
        DisposeAsync(false).AsTask();
    }
}
