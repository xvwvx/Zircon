// Licensed to the X.

namespace Mir3.Shared.Core;

public readonly struct Association
{
    public readonly int Id;
    public readonly int Value;

    public Association(int id, int value)
    {
        Id = id;
        Value = value;
    }
}
