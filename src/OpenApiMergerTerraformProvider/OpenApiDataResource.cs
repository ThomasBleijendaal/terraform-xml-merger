using System.ComponentModel;
using System.Text.Json.Serialization;
using MessagePack;
using TerraformPluginDotNet.Resources;

namespace OpenApiMergerTerraformProvider;

[SchemaVersion(1)]
[MessagePackObject]
public class OpenApiDataResource
{
    [Key("open_api_json")]
    [JsonPropertyName("open_api_json")]
    [Description("OpenApi JSON")]
    [Computed]
    public string OpenApiJson { get; set; } = null!;

    [Key("operations")]
    [JsonPropertyName("operations")]
    [Description("Operations")]
    [Computed]
    public Dictionary<string, Dictionary<string, object>> Operations { get; set; } = null!;
}
