using System.Runtime.CompilerServices;
using MemoryPack;
using Mir3.Shared.Formatters;
using Stride.Core.Mathematics;

namespace Mir3.Test;

[MemoryPackable]
public partial struct  Color41
{
    public int Rgba { get; set; }
}

internal class Program
{
    private static void Main()
    {
        MemoryPackFormatterProvider.Register<Color4>(new Color4Formatter());

        var maxValue = int.MaxValue;
        var s1 = Vector4.Zero;
        var s2 = Int2.Zero;
        var s3 = Color4.Black;
        var rgba = s3.ToRgba();
        var s4 = Unsafe.As<int, Color41>(ref rgba);
        var array0 = MemoryPackSerializer.Serialize(s4);

        var options = new MemoryPackSerializerOptions() { };
        var array1 = MemoryPackSerializer.Serialize(new Point());
        var array2 = MemoryPackSerializer.Serialize(new System.Drawing.Point());
        var array3 = MemoryPackSerializer.Serialize(s3);
        var array4 = MemoryPackSerializer.Serialize(new System.Numerics.Vector4());
    }
}
