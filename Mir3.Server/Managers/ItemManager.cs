using Autofac;
using Mir3.Shared.Models;

namespace Mir3.Server.Managers;

public sealed class ItemManager
{
    public ItemManager(IComponentContext context)
    {
        Items = DataHelper.Array<Item>(context);
        ItemSets = DataHelper.Array<ItemSet>(context);
    }

    private Item?[] Items { get; set; }
    private ItemSet?[] ItemSets { get; set; }
}
