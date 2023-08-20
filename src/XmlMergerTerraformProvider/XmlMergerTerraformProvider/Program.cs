using TerraformPluginDotNet;
using TerraformPluginDotNet.ResourceProvider;
using XmlMergerTerraformProvider;

await TerraformPluginHost.RunAsync(args, "thomas-ict.nl/azure/xmlmerger", (services, registry) =>
{
    services.AddSingleton<PluginConfigurator>();
    services.AddTerraformProviderConfigurator<PluginConfiguration, PluginConfigurator>();
    services.AddTransient<IDataSourceProvider<XmlPolicy>, XmlPolicyDataSourceProvider>();
    services.AddTransient<IDataSourceSchemaProvider, XmlFileSchemaProvider>();

});

/* plugin todos
 * 
 * - remove serilog json file requirement
 * - document TF_XML_POLICIES_FOLDER + TF_XML_BASE_POLICY
 * 
 * xml todos
 * 
 * - support for "placement = top"
 * - support for merging into specific parent element (all whens into a choose)
 * 
*/
