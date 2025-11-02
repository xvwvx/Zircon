using Autofac;
using Mir3.Server.Bootstrap;
using Mir3.Server.Managers;

namespace Mir3.Server;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule(new AutofacModule());
        var container = builder.Build();

        var itemManager = container.Resolve<ItemManager>();
    }
}
