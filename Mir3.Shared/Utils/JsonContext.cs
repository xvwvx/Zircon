// Licensed to the X.

using System.Text.Json.Serialization;
using Library;
using Mir3.Shared.Models;

namespace Mir3.Shared.Utils;

[JsonSerializable(typeof(Stats))]
[JsonSerializable(typeof(ItemSetStats[]))]
public sealed partial class JsonContext : JsonSerializerContext
{
}
