// Licensed to X.

namespace Metal.Utils;

public abstract class DisposableBase : IDisposable
{
    private bool _disposed;

    protected DisposableBase()
    {
        _disposed = false;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~DisposableBase()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // 释放托管资源
                DisposeManagedResources();
            }

            // 释放非托管资源
            DisposeUnmanagedResources();
        }

        _disposed = true;
    }

    protected virtual void DisposeManagedResources()
    {
    }

    protected virtual void DisposeUnmanagedResources()
    {
    }
}
