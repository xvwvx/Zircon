// Licensed to the X.


using Mir3.Shared;

namespace Metal.Data;

public abstract class BaseRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    public abstract uint NextId();

    public abstract TEntity? Get(uint id);

    public abstract IEnumerable<TEntity> GetAll();

    public abstract void Create(TEntity data);

    public abstract void BulkCreate(IEnumerable<TEntity> enumerable);

    public abstract void Update(TEntity data);

    public abstract void Upsert(TEntity data);

    public abstract bool Delete(uint id);

    public abstract bool Exists(uint id);

    public void Update(object data)
    {
        Update((TEntity)data);
    }
}
