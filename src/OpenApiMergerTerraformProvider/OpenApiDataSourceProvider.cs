using TerraformPluginDotNet.ResourceProvider;

namespace OpenApiMergerTerraformProvider;

public class OpenApiDataSourceProvider : IDataSourceProvider<OpenApiDataResource>
{
    public Task<OpenApiDataResource> ReadAsync(OpenApiDataResource request)
    {
        return Task.FromResult(request);
    }
}
