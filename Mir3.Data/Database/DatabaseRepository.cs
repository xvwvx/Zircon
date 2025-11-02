// Licensed to the X.

using System.Linq.Expressions;
using Mir3.Data.Models;

namespace Mir3.Data.Database;

public class DatabaseRepository<TEntity> : BaseRepository<TEntity> where TEntity : BaseEntity
{
    private readonly IFreeSql _freeSql;
    private uint _nextId;

    public DatabaseRepository(IFreeSql freeSql)
    {
        _freeSql = freeSql;
        _nextId = (uint)(_freeSql.Select<TEntity>().Max(entity => (long?)entity.Id) ?? 0);
    }

    public override uint NextId()
    {
        return Interlocked.Increment(ref _nextId);
    }

    public override TEntity? Get(uint id)
    {
        return _freeSql.Select<TEntity>().Where(v => v.Id == id).First();
    }

    public override IEnumerable<TEntity> GetAll()
    {
        return _freeSql.Select<TEntity>().ToList();
    }

    public override void Create(TEntity entity)
    {
        var now = DateTime.UtcNow;
        entity.CreatedAt = now;
        entity.UpdatedAt = now;
        _freeSql.Insert<TEntity>()
            .AppendData(entity)
            .ExecuteAffrows();
    }

    public override void BulkCreate(IEnumerable<TEntity> enumerable)
    {
        var now = DateTime.UtcNow;
        var entities = enumerable as TEntity[] ?? enumerable.ToArray();
        foreach (var entity in entities)
        {
            entity.CreatedAt = now;
            entity.UpdatedAt = now;
        }

        _freeSql.Insert<TEntity>()
            .AppendData(entities)
            .ExecuteAffrows();
    }

    public override void Update(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _freeSql.Update<TEntity>(entity);
    }

    public override void Upsert(TEntity entity)
    {
        var now = DateTime.UtcNow;
        if (entity.CreatedAt == default)
        {
            entity.CreatedAt = now;
        }

        entity.UpdatedAt = now;

        _freeSql.InsertOrUpdate<TEntity>()
            .SetSource(entity)
            .ExecuteAffrows();
    }

    public override bool Delete(uint id)
    {
        var rows = _freeSql.Update<TEntity>()
            .Where(v => v.Id == id)
            .Set(v => v.DeletedAt, DateTime.UtcNow)
            .ExecuteAffrows();
        return rows > 0;
    }

    public override bool Exists(uint id)
    {
        return _freeSql.Select<TEntity>().Where(v => v.Id == id).Any();
    }

    public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> exp)
    {
        return _freeSql.Select<TEntity>().Where(exp).ToList();
    }
}
