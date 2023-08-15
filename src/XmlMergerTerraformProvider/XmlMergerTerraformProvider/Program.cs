﻿using TerraformPluginDotNet;
using TerraformPluginDotNet.ResourceProvider;
using XmlMergerTerraformProvider;

await TerraformPluginHost.RunAsync(args, "thomas-ict.nl/azure/xmlmerger", (services, registry) =>
{
    services.AddSingleton<AzureConfigurator>();
    services.AddTerraformProviderConfigurator<AzureConfiguration, AzureConfigurator>();
    services.AddTransient<IDataSourceProvider<XmlPolicy>, XmlPolicyDataSourceProvider>();
    services.AddTransient<IDataSourceSchemaProvider, XmlFileSchemaProvider>();

});

/* plugin todos
 * 
 * - remove serilog json file requirement
 * - 
 * 
 * xml todos
 * 
 * - support for "placement = top"
 * - support for merging into specific parent element (all whens into a choose)
 * 
*/
