using System.ComponentModel;
using System.Text.Json.Serialization;
using MessagePack;
using TerraformPluginDotNet.Resources;

namespace OpenApiMergerTerraformProvider;

[SchemaVersion(1)]
[MessagePackObject]
public class PluginConfiguration
{
    [Key("function_app_root_folder")]
    [JsonPropertyName("function_app_root_folder")]
    [Description("Root folder of function apps")]
    [Required]
    public string RootFolder { get; set; } = null!;
}
