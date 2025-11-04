// Licensed to the X.

namespace Mir3.Shared.Enums;

[Flags]
public enum RequiredGender : byte
{
    Male = 1,
    Female = 2,
    None = Male | Female
}
