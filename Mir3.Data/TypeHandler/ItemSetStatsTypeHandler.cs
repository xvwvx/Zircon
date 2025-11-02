// Licensed to the X.

using FreeSql.DataAnnotations;
using FreeSql.Internal.Model;
using Mir3.Data.Models;
using Mir3.Data.Utils;

namespace Mir3.Data.TypeHandler;

public class ItemSetStatsTypeHandler : TypeHandler<ItemSetStats[]>
{
    public override void FluentApi(ColumnFluent col)
    {
        col.MapType(typeof(string)).StringLength(-1);
    }

    public override ItemSetStats[] Deserialize(object value)
    {
        return JsonHelper.Deserialize<ItemSetStats[]>((string)value)!;
    }

    public override object Serialize(ItemSetStats[] value)
    {
        var text = JsonHelper.Serialize(value);
        return text;
    }
}
