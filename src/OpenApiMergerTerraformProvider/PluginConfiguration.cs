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

    [Key("function_app_env")]
    [JsonPropertyName("function_app_env")]
    [Description("Environment variables for function app")]
    public Dictionary<string, string>? EnvVariables { get; set; }

    [Key("function_app_start_retry_count")]
    [JsonPropertyName("function_app_start_retry_count")]
    [Description("Function app start retry count")]
    public int? RetryCount { get; set; }

    [Key("function_app_start_retry_delay")]
    [JsonPropertyName("function_app_start_retry_delay")]
    [Description("Function app start retry delay")]
    public int? RetryDelay { get; set; }

    [Key("function_core_tools")]
    [JsonPropertyName("function_core_tools")]
    [Description("Function core tools")]
    public string? FunctionCoreTools { get; set; }

    [Key("function_setting_example_json")]
    [JsonPropertyName("function_setting_example_json")]
    [Description("Function settings example json")]
    public string? FunctionSettingsExampleJson { get; set; }
}
