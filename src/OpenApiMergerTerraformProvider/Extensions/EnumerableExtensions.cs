using System.Collections.Specialized;
using Microsoft.OpenApi.Interfaces;

namespace OpenApiMergerTerraformProvider.Extensions;

public static class EnumerableExtensions
{
    public static void AddRange(this StringDictionary dict, IEnumerable<KeyValuePair<string, string>> items)
    {
        foreach (var item in items)
        {
            dict.Add(item.Key, item.Value);
        }
    }

    public static void AddOrSetRange<T>(this IDictionary<string, T> dict, IEnumerable<KeyValuePair<string, T>> items)
        where T : IOpenApiSerializable
    {
        foreach (var item in items)
        {
            dict[item.Key] = item.Value;
        }
    }
}
