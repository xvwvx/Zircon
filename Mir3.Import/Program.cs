using System.Reflection;
using FreeSql;
using Library;
using Library.SystemModels;
using Mir3.Shared.Database;
using Mir3.Shared.Models;
using MirDB;
using Server.DBModels;
using Server.Envir;

namespace Mir3.Import;

internal class Program
{
    private static void Main(string[] args)
    {
        var assembly = Assembly.GetAssembly(typeof(Config));
        ConfigReader.Load(assembly);
        var session = new Session(SessionMode.Users) { BackUpDelay = 60 };

        session.Initialize(
            Assembly.GetAssembly(typeof(ItemInfo)),
            Assembly.GetAssembly(typeof(AccountInfo))
        );

        var connectionString = "Host=localhost;Port=5432;Database=Mir3;Username=postgres;Password=123456";
        var freeSql = new FreeSqlBuilder()
            .UseConnectionString(DataType.PostgreSQL, connectionString)
            .UseAutoSyncStructure(true)
            .UseMonitorCommand(cmd => Console.WriteLine(cmd.CommandText))
            .Build();
        var dataSource = new DatabaseDataSource(freeSql);

        var repository = dataSource.GetRepository<Item>();
        var a1 = repository.GetAll();
        var a2 = repository.Get(2);

        try
        {
            Transform<SetInfo, ItemSet>(session, dataSource);
            Transform<ItemInfo, Item>(session, dataSource);
            Console.WriteLine("end");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static void Transform<TFrom, TTo>(Session session, DatabaseDataSource dataSource)
        where TFrom : DBObject, new()
        where TTo : class, new()
    {
        var collection = session.GetCollection<TFrom>();
        var array = TransformHelper.To<TFrom, TTo>(collection);
        // var repository = dataSource.GetRepository<TTo>();
        // foreach (var entity in array)
        // {
        //     repository.Upsert(entity);
        // }
    }
}
