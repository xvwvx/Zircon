// Licensed to X.

using Arch.Core;
using Autofac;
using Mir3.Data.Database;
using Mir3.Shared.ECS;
using Mir3.Shared.Utils;
using Serilog;

namespace Mir3.Server.Services;

public sealed class DbService
    : BaseSystem<World, UpdateTick>
{
    private readonly ILogger _logger;
    public readonly DatabaseDataSource DataSource;

    private readonly Queue<Action<DatabaseDataSource>> _queue = new();

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
