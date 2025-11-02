// Licensed to X.

using System.Diagnostics;
using Autofac;
using FreeSql;
using Microsoft.Extensions.Configuration;
using Mir3.Data.Database;
using Mir3.Data.Models;
using Mir3.Server.Managers;
using Serilog;

namespace Mir3.Server.Bootstrap;

public sealed class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        LoadCommon(builder);
        LoadManagers(builder);
        LoadServices(builder);
        LoadSystems(builder);
    }

    private static void LoadCommon(ContainerBuilder builder)
    {
        builder.Register(IConfiguration (c) =>
            {
                var isDebug = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES") == "debug";
                var environment = isDebug ? "Development" : "Production";
                return new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{environment}.json", true)
                    .Build();
            })
            .SingleInstance();

        builder.Register(ILogger (c) =>
            {
                var configuration = c.Resolve<IConfiguration>();
                return Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();
            })
            .SingleInstance();

        builder.Register(c =>
            {
                var logger = c.Resolve<ILogger>().ForContext<ILogger>();
                var configuration = c.Resolve<IConfiguration>();
                var connectionString = configuration.GetConnectionString("PostgreSQL");
                Debug.Assert(connectionString != null, nameof(connectionString) + " != null");
                var freeSql = new FreeSqlBuilder()
                    .UseConnectionString(DataType.PostgreSQL, connectionString)
                    .UseAutoSyncStructure(true)
                    .UseMonitorCommand(cmd => logger.Debug(cmd.CommandText))
                    .Build();
                return new DatabaseDataSource(freeSql);
            })
            .SingleInstance();
    }

    private static void LoadManagers(ContainerBuilder builder)
    {
        builder.Register(c => new ItemManager(c))
            .SingleInstance();
    }

    private static void LoadServices(ContainerBuilder builder)
    {
    }

    private static void LoadSystems(ContainerBuilder builder)
    {
    }
}
