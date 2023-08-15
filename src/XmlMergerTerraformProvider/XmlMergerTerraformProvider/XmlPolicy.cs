using System.ComponentModel;
using System.Text.Json.Serialization;
using MessagePack;
using TerraformPluginDotNet.Resources;
using TerraformPluginDotNet.Serialization;

namespace XmlMergerTerraformProvider;

[SchemaVersion(1)]
[MessagePackObject]
public class XmlPolicy
{
    [Key("policy_name")]
    [JsonPropertyName("policy_name")]
    [Description("Name of the policy file")]
    [Computed]
    [MessagePackFormatter(typeof(ComputedStringValueFormatter))]
    public string? PolicyName { get; set; } = null!;
}
