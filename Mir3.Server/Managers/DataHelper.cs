// Licensed to the X.

using Autofac;
using Mir3.Data.Database;
using Mir3.Data.Models;

namespace Mir3.Server.Managers;

public static class DataHelper
{
    public static TEntity?[] Array<TEntity>(IComponentContext context)
        where TEntity : BaseEntity
    {
        var repository = context.Resolve<DatabaseDataSource>().GetRepository<TEntity>();
        var entities = repository.GetAll().ToArray();
        var array = new TEntity?[entities.Max(v => v.Id) + 1];
        foreach (var entity in entities)
        {
            array[entity.Id] = entity;
        }

        return array;
    }
}
