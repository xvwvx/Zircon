// Licensed to the X.

using MemoryPack;
using Stride.Core.Mathematics;

namespace Mir3.Shared.Formatters;

public sealed class Color4Formatter : MemoryPackFormatter<Color4>
{
    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Color4 value)
    {
        writer.WriteVarInt(value.ToRgba());
    }

    public override void Deserialize(ref MemoryPackReader reader, scoped ref Color4 value)
    {
        var rgba = reader.ReadVarIntInt32();
        value = new Color4(rgba);
    }
}
