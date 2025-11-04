// Licensed to the X.


using FreeSql.DataAnnotations;
using Metal.Data;

namespace Mir3.Shared.Models;

[Index($"uk_{nameof(Account)}_{nameof(Password)}", $"{nameof(Account)},{nameof(Password)}", true)]
public sealed class AccountModel : IHasDataId, IAuditable
{
    [Column(IsPrimary = true, IsIdentity = true)]
    public uint Id { get; set; }

    public string Account { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public int GmLevel { get; set; }

    public string LastIp { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    [Navigate(nameof(CharacterModel.AccountId))]
    public List<CharacterModel> Characters { get; set; } = [];
}
