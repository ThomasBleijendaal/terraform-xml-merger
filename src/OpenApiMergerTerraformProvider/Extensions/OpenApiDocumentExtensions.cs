using Microsoft.OpenApi.Interfaces;

namespace OpenApiMergerTerraformProvider.Extensions;

public static class OpenApiDocumentExtensions
{
    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            list.Add(item);
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
