// Licensed to the X.

using System.Runtime.InteropServices;
using Mir3.Shared;

namespace Metal.Data;

public abstract class RepositoryDataSource : BaseDataSource
{
    private readonly Dictionary<Type, IRepository> _repositories = new();

    protected abstract IRepository<T> CreateRepository<T>() where T : class;

    public IRepository<T> GetRepository<T>() where T : class
    {
        var type = typeof(T);
        ref var repository = ref CollectionsMarshal.GetValueRefOrAddDefault(_repositories, type, out var exists);
        if (!exists)
        {
            repository = CreateRepository<T>();
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
        var repository = _repositories[entity.GetType()];
        repository.Update(entity);
    }
}
