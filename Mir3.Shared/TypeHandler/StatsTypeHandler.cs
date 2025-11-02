// Licensed to the X.

using FreeSql.DataAnnotations;
using FreeSql.Internal.Model;
using Library;
using Mir3.Shared.Utils;

namespace Mir3.Shared.TypeHandler;

public class StatsTypeHandler : TypeHandler<Stats>
{
    public override void FluentApi(ColumnFluent col)
    {
        col.MapType(typeof(string)).StringLength(-1);
    }

    public override Stats Deserialize(object value)
    {
        var dict = JsonHelper.Deserialize<SortedDictionary<Stat, int>>((string)value);
        return new Stats { Values = dict };
    }

    public override object Serialize(Stats value)
    {
        var text = JsonHelper.Serialize(value.Values);
        return text;
    }
}
