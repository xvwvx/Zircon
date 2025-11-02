// Licensed to X.

using System.Diagnostics;

namespace Mir3.Shared.Utils;

public abstract class TypedEventDispatcher<TSession, TBase> where TSession : class where TBase : class
{
    public delegate void SyncDelegate<in TData>(
        TSession session,
        TData packet
    ) where TData : TBase;

    private readonly Dictionary<Type, SyncDelegate<TBase>> _handlers
        = new(256);

    protected void RegisterInternal(Type type, SyncDelegate<TBase> handler)
    {
        if (!_handlers.TryAdd(type, handler))
        {
            throw new InvalidOperationException($"重复注册消息 type={type}");
        }
    }

    protected void UnregisterInternal(Type type)
    {
        _handlers.Remove(type);
    }

    private readonly struct SyncHandlerWrapper<TData>(SyncDelegate<TData> handler)
        where TData : TBase
    {
        public void Invoke(TSession session, TBase data)
        {
            handler(session, (TData)data);
        }
    }

    public void Register<TData>(SyncDelegate<TData> handler) where TData : TBase
    {
        var wrapper = new SyncHandlerWrapper<TData>(handler);
        RegisterInternal(TypeCache<TData>.Type, wrapper.Invoke);
    }

    public void Unregister<TData>(SyncDelegate<TData> handler) where TData : TBase
    {
        UnregisterInternal(typeof(TData));
    }

    public delegate Task AsyncDelegate<in TData>(
        TSession session,
        TData packet
    ) where TData : TBase;

    private readonly struct AsyncHandlerWrapper<TData>(AsyncDelegate<TData> handler)
        where TData : TBase
    {
        public void Invoke(TSession session, TBase data)
        {
            _ = handler(session, (TData)data);
        }
    }

    public void Register<TData>(AsyncDelegate<TData> handler) where TData : TBase
    {
        var wrapper = new AsyncHandlerWrapper<TData>(handler);
        RegisterInternal(TypeCache<TData>.Type, wrapper.Invoke);
    }

    public void Unregister<TData>(AsyncDelegate<TData> handler) where TData : TBase
    {
        UnregisterInternal(TypeCache<TData>.Type);
    }

    public bool Dispatch(TSession session, in TBase data)
    {
        Debug.Assert(data != null);
        var type = data.GetType();
        if (_handlers.TryGetValue(type, out var handler))
        {
            handler(session, data);
            return true;
        }

        return false;
    }

    public bool Dispatch<TData>(TSession session, TData data) where TData : TBase
    {
        if (_handlers.TryGetValue(TypeCache<TData>.Type, out var handler))
        {
            handler(session, data);
            return true;
        }

        return false;
    }
}
