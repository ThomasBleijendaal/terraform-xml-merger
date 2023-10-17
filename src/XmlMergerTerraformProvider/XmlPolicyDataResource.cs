using System.ComponentModel;
using System.Text.Json.Serialization;
using MessagePack;
using TerraformPluginDotNet.Resources;
using TerraformPluginDotNet.Serialization;

namespace XmlMergerTerraformProvider;

[SchemaVersion(1)]
[MessagePackObject]
public record XmlPolicyDataResource
{
    [Key("base_xml")]
    [JsonPropertyName("base_xml")]
    [Description("Base xml to merge all fragments into")]
    public string? BaseXml { get; set; }

    [Key("policy_name")]
    [JsonPropertyName("policy_name")]
    [Description("Logical name for the compiled policy xml")]
    [Required]
    public string PolicyName { get; set; } = null!;

    [Key("fragments")]
    [JsonPropertyName("fragments")]
    [Description("Fragments to combine")]
    public List<Dictionary<string, Dictionary<string, object>>>? Fragments { get; set; }

    [Key("named_values")]
    [JsonPropertyName("named_values")]
    [Description("Used named values in the compiled policy xml, keys are original named value name, the value is the name used in the compiled policy xml")]
    [Computed]
    public Dictionary<string, string> UsedNamedValues { get; set; } = null!;

    [Key("xml")]
    [JsonPropertyName("xml")]
    [Description("Raw xml output")]
    [Computed]
    [MessagePackFormatter(typeof(ComputedStringValueFormatter))]
    public string? Xml { get; set; }
}
