using TerraformPluginDotNet.ProviderConfig;

namespace XmlMergerTerraformProvider;

public class PluginConfigurator : IProviderConfigurator<PluginConfiguration>
{
    private PluginConfiguration? _config;

    public Task ConfigureAsync(PluginConfiguration config)
    {
        _config = config;

        return Task.CompletedTask;
    }

    public PluginConfiguration Config => _config ?? new PluginConfiguration
    {
        PolicyFolder = Environment.GetEnvironmentVariable("TF_XML_POLICIES_FOLDER")
            ?? Path.Combine(Directory.GetCurrentDirectory(), "policies"),
        BasePolicy = Environment.GetEnvironmentVariable("TF_XML_BASE_POLICY")
            ?? Path.Combine(Directory.GetCurrentDirectory(), "base", "base.xml")
    };
}
