// Licensed to the X.

namespace Mir3.Shared.Enums;

[Flags]
public enum RequiredClass : byte
{
    None = 0,
    Warrior = 1,
    Wizard = 2,
    Taoist = 4,
    Assassin = 8,

    WarWizTao = Warrior | Wizard | Taoist,
    WizTao = Wizard | Taoist,
    AssWar = Warrior | Assassin,
    All = WarWizTao | Assassin
}
