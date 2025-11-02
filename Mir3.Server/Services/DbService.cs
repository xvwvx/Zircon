// Licensed to X.

using Arch.Core;
using Autofac;
using Metal.Utils;
using Mir3.Shared.Database;
using Mir3.Shared.ECS;
using Serilog;

namespace Mir3.Server.Services;

public sealed class DbService
    : BaseSystem<World, UpdateTick>
{
    private readonly ILogger _logger;

    private readonly Queue<Action<DatabaseDataSource>> _queue = new();
    public readonly DatabaseDataSource DataSource;

    public DbService(IComponentContext context) : base(context)
    {
        _logger = context.Resolve<ILogger>().ForContext<DbService>();
        DataSource = context.Resolve<DatabaseDataSource>();
    }

    public override void Initialize()
    {
    }

    public override void AfterUpdate(UpdateTick tick)
    {
        while (_queue.TryDequeue(out var action))
        {
            action(DataSource);
        }
    }

    public override void Clear()
    {
        while (_queue.TryDequeue(out var action))
        {
            action(DataSource);
        }
    }

    public void Execute(Action<DatabaseDataSource> action)
    {
        _queue.Enqueue(action);
    }
}
