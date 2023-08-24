﻿using System.ComponentModel;
using System.Text.Json.Serialization;
using MessagePack;
using TerraformPluginDotNet.Resources;

namespace XmlMergerTerraformProvider;

// TODO: this does not really work - schema is requested too early compared to configuring this

[SchemaVersion(1)]
[MessagePackObject]
public class PluginConfiguration
{
    [Key("policy_folder")]
    [JsonPropertyName("policy_folder")]
    [Description("Folder containing all policies")]
    public string PolicyFolder { get; set; } = null!;

    [Key("base_policy")]
    [JsonPropertyName("base_policy")]
    [Description("Base policy to merge all fragments into")]
    public string BasePolicy { get; set; } = null!;
}
