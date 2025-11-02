// Licensed to the X.

using Autofac;
using Metal.Data;
using Mir3.Shared.Database;

namespace Mir3.Server.Managers;

public static class DataHelper
{
    public static TEntity?[] Array<TEntity>(IComponentContext context)
        where TEntity : class, IHasDataId
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
