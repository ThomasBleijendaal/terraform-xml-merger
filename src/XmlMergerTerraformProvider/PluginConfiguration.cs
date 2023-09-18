using System.ComponentModel;
using System.Text.Json.Serialization;
using MessagePack;
using TerraformPluginDotNet.Resources;

namespace XmlMergerTerraformProvider;

[SchemaVersion(1)]
[MessagePackObject]
public class PluginConfiguration
{
    [Key("policy_folder")]
    [JsonPropertyName("policy_folder")]
    [Description("Folder containing all policies")]
    [Computed]
    public string PolicyFolder { get; set; } = null!;

    [Key("base_policy")]
    [JsonPropertyName("base_policy")]
    [Description("Base policy to merge all fragments into")]
    [Computed]
    public string BasePolicy { get; set; } = null!;
}
