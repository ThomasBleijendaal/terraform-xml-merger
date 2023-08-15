using System.Collections.Immutable;
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
        var replaceFragment = new Dictionary<string, TerraformType>
        {
            ["from"] = _terraformTypeBuilder.GetTerraformType(typeof(string)),
            ["to"] = _terraformTypeBuilder.GetTerraformType(typeof(string)),
        };
        var replaceFragmentType = new TerraformType.TfObject(replaceFragment.ToImmutableDictionary(), ImmutableHashSet<string>.Empty);

        var setHeader = new Dictionary<string, TerraformType>
        {
            ["value"] = _terraformTypeBuilder.GetTerraformType(typeof(int))
        };
        var setHeaderType = new TerraformType.TfObject(setHeader.ToImmutableDictionary(), ImmutableHashSet<string>.Empty);

        var fragments = new Dictionary<string, TerraformType>
        {
            ["set_header"] = setHeaderType,
            ["replace"] = replaceFragmentType
        };
        var fragmentsType = new TerraformType.TfObject(fragments.ToImmutableDictionary(), fragments.Keys.ToImmutableHashSet());

        var policies = GetBasePolicySchema();

        policies.Block.Attributes.Add(new Schema.Types.Attribute
        {
            Description = "Fragments to combine",
            DescriptionKind = StringKind.Plain,
            Name = "fragments",
            Required = true,
            Type = ByteString.CopyFromUtf8(fragmentsType.ToJson())
        });

        yield return new DataSourceRegistration("xmlmerger_policy", typeof(XmlPolicy), policies);
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
                        DescriptionKind = StringKind.Plain,
                        Name = "policy_name",
                        Type = ByteString.CopyFromUtf8(terraformStringType.ToJson())
                    },
                    new Schema.Types.Attribute
                    {
                        Computed = true,
                        Description = "Raw xml output",
                        DescriptionKind = StringKind.Plain,
                        Name = "xml",
                        Type = ByteString.CopyFromUtf8(terraformStringType.ToJson())
                    }
                }
            }
        };
    }

}
