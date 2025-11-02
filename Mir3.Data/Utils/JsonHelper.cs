// Licensed to the X.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mir3.Data.Utils;

public sealed class JsonHelper
{
    private static readonly JsonSerializerOptions _options = new()
    {
        TypeInfoResolver = JsonContext.Default, Converters = { new JsonStringEnumConverter() }
    };

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, _options);
    }

    public static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, _options);
    }
}
