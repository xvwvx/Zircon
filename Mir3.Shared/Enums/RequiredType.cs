// Licensed to the X.

namespace Mir3.Shared.Enums;

[Flags]
public enum RequiredType : byte
{
    Level,
    MaxLevel,
    AC,
    MR,
    DC,
    MC,
    SC,
    Health,
    Mana,
    Accuracy,
    Agility,
    CompanionLevel,
    MaxCompanionLevel,
    RebirthLevel,
    MaxRebirthLevel
}
