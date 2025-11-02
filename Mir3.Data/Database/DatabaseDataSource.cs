// Licensed to the X.

using System.Runtime.InteropServices;
using Library;
using Mir3.Data.Models;
using Mir3.Data.TypeHandler;

namespace Mir3.Data.Database;

public class DatabaseDataSource : BaseDataSource
{
    public readonly Dictionary<Type, IRepository> Repositories = new();

    static DatabaseDataSource()
    {
        FreeSql.Internal.Utils.TypeHandlers.TryAdd(typeof(Stats), new StatsTypeHandler());
        FreeSql.Internal.Utils.TypeHandlers.TryAdd(typeof(ItemSetStats[]), new ItemSetStatsTypeHandler());
    }

    public DatabaseDataSource(IFreeSql connection)
    {
        connection.GlobalFilter
            .Apply<BaseEntity>("DeletedAt", v => v.DeletedAt == null);
        connection.UseJsonMap();
        Connection = connection;
    }

    public IFreeSql Connection { get; }

    public IRepository<T> GetRepository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        ref var repository = ref CollectionsMarshal.GetValueRefOrAddDefault(Repositories, type, out var exists);
        if (!exists)
        {
            repository = new DatabaseRepository<T>(Connection);
        }

        return (repository as IRepository<T>)!;
    }

    public override TEntity? Get<TEntity>(uint id) where TEntity : class
    {
        return GetRepository<TEntity>().Get(id);
    }

    public override IEnumerable<TEntity> GetAll<TEntity>()
    {
        return GetRepository<TEntity>().GetAll();
    }

    public override void Create<TEntity>(TEntity entity)
    {
        GetRepository<TEntity>().Create(entity);
    }

    public override void BulkCreate<TEntity>(IEnumerable<TEntity> enumerable)
    {
        GetRepository<TEntity>().BulkCreate(enumerable);
    }

    public override void Update<TEntity>(TEntity entity)
    {
        GetRepository<TEntity>().Update(entity);
    }

    public override void Upsert<TEntity>(TEntity entity)
    {
        GetRepository<TEntity>().Upsert(entity);
    }

    public override bool Delete<TEntity>(uint id)
    {
        return GetRepository<TEntity>().Delete(id);
    }

    public override bool Exists<TEntity>(uint id)
    {
        return GetRepository<TEntity>().Exists(id);
    }

    public override void Update(object entity)
    {
        var repository = Repositories[entity.GetType()];
        repository.Update(entity);
    }
}
