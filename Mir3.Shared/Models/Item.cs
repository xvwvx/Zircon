// Licensed to the X.

using FreeSql.DataAnnotations;
using Library;
using Metal.Data;

namespace Mir3.Shared.Models;

// [Index("uk_ItemName", "ItemName", true)]
public sealed class Item : IHasDataId, IAuditable
{
    public string ItemName { get; set; } = string.Empty;

    [Column(MapType = typeof(string))]
    public ItemType ItemType { get; set; }

    [Column(MapType = typeof(string))]
    public RequiredClass RequiredClass { get; set; }

    [Column(MapType = typeof(string))]
    public RequiredGender RequiredGender { get; set; }

    [Column(MapType = typeof(string))]
    public RequiredType RequiredType { get; set; }

    public int RequiredAmount { get; set; }
    public int Shape { get; set; }

    [Column(MapType = typeof(string))]
    public ItemEffect ItemEffect { get; set; }

    [Column(MapType = typeof(string))]
    public ExteriorEffect ExteriorEffect { get; set; }

    public int Image { get; set; }
    public int Durability { get; set; }
    public int Price { get; set; }
    public int Weight { get; set; }
    public int StackSize { get; set; }
    public bool StartItem { get; set; }
    public decimal SellRate { get; set; }
    public bool CanRepair { get; set; }
    public bool CanSell { get; set; }
    public bool CanStore { get; set; }
    public bool CanTrade { get; set; }
    public bool CanDrop { get; set; }
    public bool CanDeathDrop { get; set; }
    public string Description { get; set; } = string.Empty;

    [Column(MapType = typeof(string))]
    public Rarity Rarity { get; set; }

    public bool CanAutoPot { get; set; }
    public int BuffIcon { get; set; }
    public int PartCount { get; set; }
    public uint Set { get; set; }

    public Stats Stats { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    [Column(IsPrimary = true, Position = short.MaxValue)]
    public uint Id { get; set; }
}
