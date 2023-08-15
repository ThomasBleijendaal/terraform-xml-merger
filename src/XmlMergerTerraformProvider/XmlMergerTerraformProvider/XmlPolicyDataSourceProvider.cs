using System.Text.Json;
using TerraformPluginDotNet.ResourceProvider;

namespace XmlMergerTerraformProvider;

public class XmlPolicyDataSourceProvider : IDataSourceProvider<XmlPolicy>
{
    public XmlPolicyDataSourceProvider()
    {

    }

    public Task<XmlPolicy> ReadAsync(XmlPolicy request)
    {
        var output = new XmlPolicy
        {
            PolicyName = "Abc",
            Fragments = request.Fragments,
            Xml = JsonSerializer.Serialize(request.Fragments)
        };

        return Task.FromResult(output);
    }
}
