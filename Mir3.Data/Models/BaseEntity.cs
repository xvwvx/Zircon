// Licensed to X.

using FreeSql.DataAnnotations;

namespace Mir3.Data.Models;

public abstract class BaseEntity
{
    [Column(IsPrimary = true, Position = short.MaxValue)]
    public uint Id { get; set; }

    [Column(Position = short.MinValue)]
    public DateTime CreatedAt { get; set; }

    [Column(Position = short.MinValue)]
    public DateTime UpdatedAt { get; set; }

    [Column(Position = short.MinValue)]
    public DateTime? DeletedAt { get; set; }
}
