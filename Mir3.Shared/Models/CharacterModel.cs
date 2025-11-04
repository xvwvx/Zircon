// Licensed to the X.


using FreeSql.DataAnnotations;
using Metal.Data;
using Mir3.Shared.Enums;

namespace Mir3.Shared.Models;

[Index($"uk_{nameof(Name)}", nameof(Name), true)]
public sealed class CharacterModel : IHasDataId, IAuditable
{
    [Column(IsPrimary = true, IsIdentity = true)]
    public uint Id { get; set; }

    public uint AccountId { get; set; }

    public string Name { get; set; } = string.Empty;

    public MirClass Class { get; set; }
    public MirGender Gender { get; set; }
    public int Level { get; set; }
    public int HairType { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    [Navigate(nameof(AccountId))]
    public AccountModel Account { get; set; } = null!;
}
