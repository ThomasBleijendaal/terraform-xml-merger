using System.Collections.Immutable;
using Google.Protobuf;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Schemas;
using TerraformPluginDotNet.Schemas.Types;
using Tfplugin5;

namespace XmlMergerTerraformProvider;

public class XmlFileSchemaProvider : IDataSourceSchemaProvider
{
    private readonly PluginConfigurator _config;
    private readonly ILogger<XmlFileSchemaProvider> _logger;
    private readonly ITerraformTypeBuilder _terraformTypeBuilder;
    private readonly ISchemaBuilder _schemaBuilder;

    public XmlFileSchemaProvider(
        PluginConfigurator config,
        ILogger<XmlFileSchemaProvider> logger,
        ITerraformTypeBuilder terraformTypeBuilder,
        ISchemaBuilder schemaBuilder)
    {
        _config = config;
        _logger = logger;
        _terraformTypeBuilder = terraformTypeBuilder;
        _schemaBuilder = schemaBuilder;
    }

    public IEnumerable<DataSourceRegistration> GetSchemas()
    {
        var policyFiles = Directory.GetFiles(_config.Config.PolicyFolder, "*.xml");

        var fragments = new Dictionary<string, TerraformType>();

        foreach (var policyFile in policyFiles)
        {
            _logger.LogInformation("Analyzing {policyFile}", policyFile);

            try
            {
                var fragment = XmlFileHelper.ResolveParametersFromXmlFile(policyFile)
                    .ToImmutableDictionary(x => x.Key, x => _terraformTypeBuilder.GetTerraformType(x.Value));

                var fragmentType = new TerraformType.TfObject(fragment, ImmutableHashSet<string>.Empty);

                fragments.Add(Path.GetFileName(policyFile).Replace(".xml", ""), fragmentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load {policyFile}", policyFile);
            }
        }

        var fragmentsType = new TerraformType.TfObject(fragments.ToImmutableDictionary(), fragments.Keys.ToImmutableHashSet());
        var fragmentsArray = new TerraformType.TfList(fragmentsType);

        yield return new DataSourceRegistration("xmlmerger_policy", typeof(XmlPolicyDataResource), GetPolicySchema(fragmentsArray));
    }

    private Schema GetPolicySchema(TerraformType fragmentsType)
    {
        var schema = _schemaBuilder.BuildSchema(typeof(XmlPolicyDataResource));

        schema.Block.Attributes.First(x => x.Name == "fragments").Type = ByteString.CopyFromUtf8(fragmentsType.ToJson());

        return schema;
    }
}
