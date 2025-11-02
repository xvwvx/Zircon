// Licensed to the X.

using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Mir3.Data.Models;
using MirDB;

namespace Mir3.Import;

public static class TransformHelper
{
    public static TTo[] To<TFrom, TTo>(DBCollection<TFrom> collection)
        where TFrom : DBObject, new()
        where TTo : new()
    {
        return collection.Binding.Select(To<TFrom, TTo>).ToArray();
    }

    public static TTo[] To<TFrom, TTo>(DBBindingList<TFrom> collection)
        where TFrom : DBObject, new()
        where TTo : new()
    {
        return collection.Select(To<TFrom, TTo>).ToArray();
    }

    public static TTo[] To<TFrom, TTo>(IList<TFrom> list)
        where TTo : new()
    {
        return list.Select(To<TFrom, TTo>).ToArray();
    }

    public static uint[] ToIds<TFrom>(DBBindingList<TFrom> collection)
        where TFrom : DBObject, new()
    {
        return collection.Select(v => (uint)v.Index).ToArray();
    }

    public static TTo To<TFrom, TTo>(TFrom from) where TTo : new()
    {
        var map = Cache<TFrom, TTo>.Map;

        var to = new TTo();
        foreach (var pair in map)
        {
            if (pair.Value?.Invoke(from) is { } value)
            {
                var valueType = value.GetType();
                if (pair.Key.PropertyType != valueType)
                {
                    if (value is DBObject dbObject)
                    {
                        value = (uint)dbObject.Index;
                    }
                    else if (valueType.IsGenericType)
                    {
                        var toItemType = pair.Key.PropertyType.GetElementType()!;
                        var fromItemType = valueType.GetGenericArguments().First();

                        MethodInfo? method;
                        if (toItemType == typeof(uint))
                        {
                            method = typeof(TransformHelper)
                                .GetMethod("ToIds")?
                                .MakeGenericMethod(fromItemType);
                        }
                        else
                        {
                            method = typeof(TransformHelper)
                                .GetMethods()
                                .Where(m => m.Name == "To")
                                .Where(m => m.IsGenericMethodDefinition)
                                .Where(m => m.GetGenericArguments().Length == 2)
                                .Where(m =>
                                {
                                    var ps = m.GetParameters();
                                    return ps is [{ ParameterType.IsGenericType: true }] &&
                                           ps[0].ParameterType.GetGenericTypeDefinition() ==
                                           valueType.GetGenericTypeDefinition();
                                })
                                .FirstOrDefault()?
                                .MakeGenericMethod(fromItemType, toItemType);
                        }

                        if (method is not null)
                        {
                            value = method!.Invoke(null, [value]);
                        }
                    }
                }

                pair.Key.SetValue(to, value);
            }
        }

        if (to is BaseEntity entity && from is DBObject obj)
        {
            entity.Id = (uint)obj.Index;
        }

        return to;
    }

    private static class Cache<TFrom, TTo>
    {
        public static readonly Dictionary<PropertyInfo, Func<TFrom, object?>?> Map;

        static Cache()
        {
            Map = new Dictionary<PropertyInfo, Func<TFrom, object?>?>();
            foreach (var property in typeof(TTo).GetProperties(BindingFlags.Instance
                                                               | BindingFlags.Public
                                                               | BindingFlags.DeclaredOnly))
            {
                ref var func = ref CollectionsMarshal.GetValueRefOrAddDefault(Map, property, out var exist);
                if (exist)
                {
                    continue;
                }

                foreach (var name in new[] { $"_{property.Name}", property.Name })
                {
                    var fromField = typeof(TFrom).GetField(name,
                        BindingFlags.NonPublic | BindingFlags.Instance
                    );
                    if (fromField is not null)
                    {
                        func = from => fromField.GetValue(from);
                    }
                }

                if (func is null)
                {
                    var fromProperty = typeof(TFrom).GetProperty(property.Name);
                    foreach (var endName in new[] { "Id", "Ids" })
                    {
                        if (fromProperty is null && property.Name.EndsWith(endName))
                        {
                            var name = string.Concat(
                                property.Name.AsSpan(0, property.Name.Length - endName.Length),
                                endName == "Ids" ? "s" : ""
                            );
                            fromProperty = typeof(TFrom).GetProperty(name);
                        }
                    }

                    if (fromProperty is not null)
                    {
                        func = from => fromProperty.GetValue(from);
                    }
                }

                if (func is null)
                {
                    Debug.WriteLine("{0} not found", property.Name);
                }
            }
        }
    }
}
