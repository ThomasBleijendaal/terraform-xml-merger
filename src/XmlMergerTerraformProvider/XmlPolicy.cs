using System.ComponentModel;
using System.Text.Json.Serialization;
using MessagePack;
using TerraformPluginDotNet.Resources;
using TerraformPluginDotNet.Serialization;

namespace XmlMergerTerraformProvider;

[SchemaVersion(1)]
[MessagePackObject]
public record XmlPolicy
{
    [Key("base_xml")]
    [JsonPropertyName("base_xml")]
    [Description("Base xml to merge all fragments into")]
    public string? BaseXml { get; set; }

    [Key("fragments")]
    [JsonPropertyName("fragments")]
    [Description("Fragments to combine")]
    [Required]
    public Dictionary<string, Dictionary<string, object>> Fragments { get; set; } = null!;

    [Key("xml")]
    [JsonPropertyName("xml")]
    [Description("Raw xml output")]
    [Computed]
    [MessagePackFormatter(typeof(ComputedStringValueFormatter))]
    public string? Xml { get; set; }
}
