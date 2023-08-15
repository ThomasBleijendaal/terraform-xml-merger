using TerraformPluginDotNet.ResourceProvider;

namespace XmlMergerTerraformProvider;

public class XmlPolicyDataSourceProvider : IDataSourceProvider<XmlPolicy>
{
    public XmlPolicyDataSourceProvider()
    {

    }

    public Task<XmlPolicy> ReadAsync(XmlPolicy request)
    {
        return Task.FromResult(new XmlPolicy { PolicyName = "Abc" });
    }
}
