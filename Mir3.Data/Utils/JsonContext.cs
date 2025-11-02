// Licensed to the X.

using System.Text.Json.Serialization;
using Library;
using Mir3.Data.Models;

namespace Mir3.Data.Utils;

[JsonSerializable(typeof(Stats))]
[JsonSerializable(typeof(ItemSetStats[]))]
public sealed partial class JsonContext : JsonSerializerContext
{
}
