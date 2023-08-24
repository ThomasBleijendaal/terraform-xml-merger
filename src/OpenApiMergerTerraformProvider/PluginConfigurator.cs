using TerraformPluginDotNet.ProviderConfig;

namespace OpenApiMergerTerraformProvider;

public class PluginConfigurator : IProviderConfigurator<PluginConfiguration>
{
    private PluginConfiguration? _config;

    public Task ConfigureAsync(PluginConfiguration config)
    {
        _config = config;

        return Task.CompletedTask;
    }

    public PluginConfiguration Config => _config ?? new PluginConfiguration();
}
