// Licensed to the X.

namespace Metal.Data;

public abstract class BaseDataSource
{
    public abstract TEntity? Get<TEntity>(uint id) where TEntity : class;

    public abstract IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class;

    public abstract void Create<TEntity>(TEntity entity) where TEntity : class;

    public abstract void BulkCreate<TEntity>(IEnumerable<TEntity> data) where TEntity : class;

    public abstract void Update<TEntity>(TEntity entity) where TEntity : class;

    public abstract void Upsert<TEntity>(TEntity entity) where TEntity : class;

    public abstract bool Delete<TEntity>(uint id) where TEntity : class;

    public abstract bool Exists<TEntity>(uint id) where TEntity : class;

    public abstract void Update(object data);
}
