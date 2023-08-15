using System.ComponentModel;
using System.Text.Json.Serialization;
using MessagePack;
using TerraformPluginDotNet.Resources;

namespace XmlMergerTerraformProvider;

[SchemaVersion(1)]
[MessagePackObject]
public class AzureConfiguration
{
    [Key("policy_folder")]
    [JsonPropertyName("policy_folder")]
    [Description("Folder containing all policies")]
    public string PolicyFolder { get; set; } = null!;
}
