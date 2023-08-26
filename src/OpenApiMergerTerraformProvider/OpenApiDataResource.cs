using System.ComponentModel;
using System.Text.Json.Serialization;
using MessagePack;
using TerraformPluginDotNet.Resources;

namespace OpenApiMergerTerraformProvider;

[SchemaVersion(1)]
[MessagePackObject]
public record OpenApiDataResource
{
    [Key("open_api_urls")]
    [JsonPropertyName("open_api_urls")]
    [Description("OpenApi URLs")]
    [Required]
    public List<string> OpenApiUrls { get; set; } = new();

    [Key("function_apps")]
    [JsonPropertyName("function_apps")]
    [Description("Function Apps")]
    [Required]
    public List<FunctionAppDataResource> FunctionApps { get; set; } = new();

    [Key("title")]
    [JsonPropertyName("title")]
    [Description("OpenApi Title")]
    [Required]
    public string Title { get; set; } = null!;

    [Key("version")]
    [JsonPropertyName("version")]
    [Description("OpenApi Version")]
    [Required]
    public string Version { get; set; } = null!;

    [Key("open_api_json")]
    [JsonPropertyName("open_api_json")]
    [Description("OpenApi JSON")]
    [Computed]
    public string OpenApiJson { get; set; } = null!;

    [SchemaVersion(1)]
    [MessagePackObject]
    public record FunctionAppDataResource
    {
        [Key("path")]
        [JsonPropertyName("path")]
        [Description("Path to the function app")]
        [Required]
        public string Path { get; set; } = null!;

        [Key("open_api_url")]
        [JsonPropertyName("open_api_url")]
        [Description("OpenApi URL")]
        [Required]
        public string OpenApiUrl { get; set; } = null!;
    }
}
