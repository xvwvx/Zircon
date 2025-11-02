// Licensed to X.

using Autofac;

namespace Mir3.Shared.ECS;

public abstract class BaseSystem<TWorld, TTick>(IComponentContext context) : IDisposable where TWorld : notnull
{
    public TWorld World { get; private set; } = context.Resolve<TWorld>();

    public virtual void Dispose()
    {
    }

    public virtual void Initialize()
    {
    }

    public virtual void BeforeUpdate(TTick tick)
    {
    }

    public virtual void Update(TTick tick)
    {
    }

    public virtual void AfterUpdate(TTick tick)
    {
    }

    public virtual void Clear()
    {
    }
}
