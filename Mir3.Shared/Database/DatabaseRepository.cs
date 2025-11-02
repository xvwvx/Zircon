// Licensed to the X.

using System.Linq.Expressions;
using Metal.Data;

namespace Mir3.Shared.Database;

public class DatabaseRepository<TEntity> : BaseRepository<TEntity> where TEntity : class
{
    private readonly IFreeSql _freeSql;
    private uint _nextId;

    public DatabaseRepository(IFreeSql freeSql)
    {
        _freeSql = freeSql;
        _nextId = (uint)(_freeSql.Select<TEntity>().Max(entity => (long?)(entity as IHasDataId)!.Id) ?? 0);
    }

    public override uint NextId()
    {
        return Interlocked.Increment(ref _nextId);
    }

    public override TEntity? Get(uint id)
    {
        return _freeSql.Select<TEntity>().Where(v => (v as IHasDataId)!.Id == id).First();
    }

    public override IEnumerable<TEntity> GetAll()
    {
        return _freeSql.Select<TEntity>().ToList();
    }

    public override void Create(TEntity entity)
    {
        var now = DateTime.UtcNow;
        if (entity is IAuditable auditable)
        {
            auditable.CreatedAt = now;
            auditable.UpdatedAt = now;
        }

        _freeSql.Insert<TEntity>()
            .AppendData(entity)
            .ExecuteAffrows();
    }

    public override void BulkCreate(IEnumerable<TEntity> enumerable)
    {
        var now = DateTime.UtcNow;
        var entities = enumerable as TEntity[] ?? enumerable.ToArray();
        if (entities is IAuditable[] array)
        {
            foreach (var auditable in array)
            {
                auditable.CreatedAt = now;
                auditable.UpdatedAt = now;
            }
        }


        _freeSql.Insert<TEntity>()
            .AppendData(entities)
            .ExecuteAffrows();
    }

    public override void Update(TEntity entity)
    {
        if (entity is IAuditable auditable)
        {
            auditable.UpdatedAt = DateTime.UtcNow;
        }

        _freeSql.Update<TEntity>(entity);
    }

    public override void Upsert(TEntity entity)
    {
        if (entity is IAuditable auditable)
        {
            var now = DateTime.UtcNow;
            if (auditable.CreatedAt == default)
            {
                auditable.CreatedAt = now;
            }

            auditable.UpdatedAt = now;
        }


        _freeSql.InsertOrUpdate<TEntity>()
            .SetSource(entity)
            .ExecuteAffrows();
    }

    public override bool Delete(uint id)
    {
        var rows = _freeSql.Update<TEntity>()
            .Where(v => (v as IHasDataId)!.Id == id)
            .Set(v => (v as IAuditable)!.DeletedAt, DateTime.UtcNow)
            .ExecuteAffrows();
        return rows > 0;
    }

    public override bool Exists(uint id)
    {
        return _freeSql.Select<TEntity>().Where(v => (v as IHasDataId)!.Id == id).Any();
    }

    public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> exp)
    {
        return _freeSql.Select<TEntity>().Where(exp).ToList();
    }
}
