// Licensed to the X.

using Mir3.Data.Models;

namespace Mir3.Data;

public abstract class BaseDataSource
{
    public abstract TEntity? Get<TEntity>(uint id) where TEntity : BaseEntity;

    public abstract IEnumerable<TEntity> GetAll<TEntity>() where TEntity : BaseEntity;

    public abstract void Create<TEntity>(TEntity entity) where TEntity : BaseEntity;

    public abstract void BulkCreate<TEntity>(IEnumerable<TEntity> data) where TEntity : BaseEntity;

    public abstract void Update<TEntity>(TEntity entity) where TEntity : BaseEntity;

    public abstract void Upsert<TEntity>(TEntity entity) where TEntity : BaseEntity;

    public abstract bool Delete<TEntity>(uint id) where TEntity : BaseEntity;

    public abstract bool Exists<TEntity>(uint id) where TEntity : BaseEntity;

    public abstract void Update(object data);
}
