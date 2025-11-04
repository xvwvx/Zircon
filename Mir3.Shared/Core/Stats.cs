// Licensed to the X.

using System.Numerics;
using System.Runtime.InteropServices;
using MemoryPack;

namespace Mir3.Shared.Core;

[MemoryPackable(GenerateType.NoGenerate)]
public sealed partial class Stats<TKey, TValue>
    where TKey : struct
    where TValue : struct, IEquatable<TValue>, IAdditionOperators<TValue, TValue, TValue>
{
    public Stats()
    {
        Data = new Dictionary<TKey, TValue>();
    }

    public Stats(Dictionary<TKey, TValue> data)
    {
        Data = data;
    }

    public Dictionary<TKey, TValue> Data { get; set; }

    public int Count => Data.Count;

    public TValue this[TKey key]
    {
        get => Data.GetValueOrDefault(key);
        set
        {
            if (value.Equals(default))
            {
                Data.Remove(key);
            }
            else
            {
                Data[key] = value;
            }
        }
    }

    public void Add(TKey key, TValue value)
    {
        ref var reference = ref CollectionsMarshal.GetValueRefOrAddDefault(Data, key, out var exists);
        reference += value;
        if (reference.Equals(default))
        {
            Data.Remove(key);
        }
    }

    public void Add(Stats<TKey, TValue> other)
    {
        foreach (var pair in other.Data)
        {
            ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(Data, pair.Key, out var exists);
            if (exists)
            {
                value += pair.Value;
                if (value.Equals(default))
                {
                    Data.Remove(pair.Key);
                }
            }
            else
            {
                value = pair.Value;
            }
        }
    }
}
