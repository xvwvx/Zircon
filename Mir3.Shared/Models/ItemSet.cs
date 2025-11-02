// Licensed to the X.

using FreeSql.DataAnnotations;
using Library;
using Metal.Data;

namespace Mir3.Shared.Models;

public sealed class ItemSetStats
{
    public Stat Stat { get; set; }
    public int Amount { get; set; }
    public RequiredClass Class { get; set; }
    public int Level { get; set; }
}

public sealed class ItemSet : IHasDataId, IAuditable
{
    public string SetName { get; set; } = string.Empty;

    public ItemSetStats[] SetStats { get; set; } = [];

    public uint[] ItemIds { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    [Column(IsPrimary = true)]
    public uint Id { get; set; }
}
