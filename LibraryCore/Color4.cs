// Licensed to the X.

using MemoryPack;

namespace LibraryCore;

[MemoryPackable]
public partial struct Color4
{
    private int _value;

    public int ToArgb()
    {
        var rgba = _value;
        var argb = rgba >> 8 | rgba << 24;
        return argb;
    }

    public byte A => (byte)(_value & 0xFF);

    public static implicit operator System.Drawing.Color(Color4 v)
    {
        var rgba = v._value;
        var argb = rgba >> 8 | rgba << 24;
        return System.Drawing.Color.FromArgb(argb);
    }

    public static implicit operator Color4(System.Drawing.Color v)
    {
        var argb = v.ToArgb();
        var rgba = argb << 8 | argb >> 24;
        return new Color4 { _value = rgba };
    }
}
