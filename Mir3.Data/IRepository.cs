// Licensed to the X.

namespace Mir3.Data;

public interface IRepository
{
    void Update(object entity);
}

public interface IRepository<TEntity> : IRepository
    where TEntity : class
{
    uint NextId();

    TEntity? Get(uint id);
    IEnumerable<TEntity> GetAll();
    void Create(TEntity entity);
    void BulkCreate(IEnumerable<TEntity> enumerable);
    void Update(TEntity entity);
    void Upsert(TEntity entity);
    bool Delete(uint id);
    bool Exists(uint id);
}
