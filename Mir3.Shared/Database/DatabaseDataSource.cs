// Licensed to the X.

using Library;
using Metal.Data;
using Mir3.Shared.Models;
using Mir3.Shared.TypeHandler;

namespace Mir3.Shared.Database;

public sealed class DatabaseDataSource : RepositoryDataSource
{
    static DatabaseDataSource()
    {
        FreeSql.Internal.Utils.TypeHandlers.TryAdd(typeof(Stats), new StatsTypeHandler());
        FreeSql.Internal.Utils.TypeHandlers.TryAdd(typeof(ItemSetStats[]), new ItemSetStatsTypeHandler());
    }

    public DatabaseDataSource(IFreeSql connection)
    {
        connection.GlobalFilter
            .Apply<IAuditable>(nameof(IAuditable.DeletedAt), v => v.DeletedAt == null);
        Connection = connection;
    }

    public IFreeSql Connection { get; }

    protected override IRepository<T> CreateRepository<T>()
    {
        return new DatabaseRepository<T>(Connection);
    }
}
