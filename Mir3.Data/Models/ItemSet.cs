// Licensed to the X.

using FreeSql.DataAnnotations;
using Library;

namespace Mir3.Data.Models;

public sealed class ItemSetStats
{
    public Stat Stat { get; set; }
    public int Amount { get; set; }
    public RequiredClass Class { get; set; }
    public int Level { get; set; }
}

public sealed class ItemSet : BaseEntity
{
    public string SetName { get; set; } = string.Empty;

    public ItemSetStats[] SetStats { get; set; } = [];

    public uint[] ItemIds { get; set; } = [];
}
