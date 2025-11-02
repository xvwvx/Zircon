using Autofac;
using Mir3.Data.Models;

namespace Mir3.Server.Managers;

public sealed class ItemManager
{
    private Item?[] Items { get; set; }
    private ItemSet?[] ItemSets { get; set; }

    public ItemManager(IComponentContext context)
    {
        Items = DataHelper.Array<Item>(context);
        ItemSets = DataHelper.Array<ItemSet>(context);
    }
}
