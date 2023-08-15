using Google.Protobuf;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Schemas.Types;
using Tfplugin5;

namespace XmlMergerTerraformProvider;

public class XmlFileSchemaProvider : IDataSourceSchemaProvider
{
    private readonly ITerraformTypeBuilder _terraformTypeBuilder;

    public XmlFileSchemaProvider(
        ITerraformTypeBuilder terraformTypeBuilder)
    {
        _terraformTypeBuilder = terraformTypeBuilder;
    }

    public IEnumerable<DataSourceRegistration> GetSchemas()
    {
        var adhoc1 = new
        {
            Test = "A"
        };
        var adhoc2 = new
        {
            Test = "A"
        };

        var terraformObjectType1 = _terraformTypeBuilder.GetTerraformType(adhoc1.GetType());
        var terraformObjectType2 = _terraformTypeBuilder.GetTerraformType(adhoc2.GetType());

        var policy1 = GetBasePolicySchema();
        policy1.Block.Attributes.Add(new Schema.Types.Attribute
        {
            Description = "The object containing all parameters for the xml file",
            DescriptionKind = Tfplugin5.StringKind.Plain,
            Name = "params",
            Type = ByteString.CopyFromUtf8(terraformObjectType1.ToJson())
        });

        yield return new DataSourceRegistration("xmlmerger_policy1", typeof(XmlPolicy), policy1);

        var policy2 = GetBasePolicySchema();
        policy1.Block.Attributes.Add(new Schema.Types.Attribute
        {
            Description = "The object containing all parameters for the xml file",
            DescriptionKind = Tfplugin5.StringKind.Plain,
            Name = "params",
            Type = ByteString.CopyFromUtf8(terraformObjectType2.ToJson())
        });

        yield return new DataSourceRegistration("xmlmerger_policy2", typeof(XmlPolicy), policy2);
    }

    private Schema GetBasePolicySchema()
    {
        var terraformStringType = _terraformTypeBuilder.GetTerraformType(typeof(string));

        return new Schema
        {
            Version = 1,
            Block = new Schema.Types.Block
            {
                Attributes =
                {
                    new Schema.Types.Attribute
                    {
                        Computed = true,
                        Description = $"The name of the policy file - updated at {DateTime.UtcNow:s}",
                        DescriptionKind = Tfplugin5.StringKind.Plain,
                        Name = "policy_name",
                        Type = ByteString.CopyFromUtf8(terraformStringType.ToJson())
                    }
                }
            }
        };
    }
}
