using System.Collections.Immutable;
using Google.Protobuf;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Schemas.Types;
using Tfplugin5;

namespace XmlMergerTerraformProvider;

public class XmlFileSchemaProvider : IDataSourceSchemaProvider
{
    private readonly PluginConfigurator _config;
    private readonly ITerraformTypeBuilder _terraformTypeBuilder;

    public XmlFileSchemaProvider(
        PluginConfigurator config,
        ITerraformTypeBuilder terraformTypeBuilder)
    {
        _config = config;
        _terraformTypeBuilder = terraformTypeBuilder;
    }

    public IEnumerable<DataSourceRegistration> GetSchemas()
    {
        var policyFiles = Directory.GetFiles(_config.Config.PolicyFolder, "*.xml");

        var fragments = new Dictionary<string, TerraformType>();

        foreach (var policyFile in policyFiles)
        {
            try
            {
                var fragment = XmlFileHelper.ResolveParametersFromXmlFile(policyFile)
                    .ToImmutableDictionary(x => x.Key, x => _terraformTypeBuilder.GetTerraformType(x.Value));

                var fragmentType = new TerraformType.TfObject(fragment, ImmutableHashSet<string>.Empty);

                fragments.Add(Path.GetFileName(policyFile).Replace(".xml", ""), fragmentType);
            }
            catch (Exception)
            {
                // nope
            }
        }

        var fragmentsType = new TerraformType.TfObject(fragments.ToImmutableDictionary(), fragments.Keys.ToImmutableHashSet());

        yield return new DataSourceRegistration("xmlmerger_policy", typeof(XmlPolicy), GetPolicySchema(fragmentsType));
    }

    private Schema GetPolicySchema(TerraformType fragmentsType)
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
                        Description = "Base xml to merge all fragments into",
                        DescriptionKind = StringKind.Plain,
                        Name = "base_xml",
                        Optional = true,
                        Type = ByteString.CopyFromUtf8(terraformStringType.ToJson())
                    },
                    new Schema.Types.Attribute
                    {
                        Description = "Fragments to combine",
                        DescriptionKind = StringKind.Plain,
                        Name = "fragments",
                        Required = true,
                        Type = ByteString.CopyFromUtf8(fragmentsType.ToJson())
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
