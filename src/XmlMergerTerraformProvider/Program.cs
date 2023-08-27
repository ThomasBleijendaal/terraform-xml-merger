using Core;
using Serilog;
using TerraformPluginDotNet;
using TerraformPluginDotNet.ResourceProvider;
using XmlMergerTerraformProvider;

try
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(new ConfigurationBuilder().AddSeqConfiguration().Build())
        .CreateBootstrapLogger();

    await TerraformPluginHost.CreateHostBuilder(args, "thomas-ict.nl/azure/xmlmerger")
        .ConfigureResourceRegistry((services, registry) =>
        {
            services.AddSingleton<PluginConfigurator>();
            services.AddTerraformProviderConfigurator<PluginConfiguration, PluginConfigurator>();
            services.AddTransient<IDataSourceProvider<XmlPolicyDataResource>, XmlPolicyDataSourceProvider>();
            services.AddTransient<IDataSourceSchemaProvider, XmlFileSchemaProvider>();
        }, 5345)
        .ConfigureAppConfiguration(builder =>
        {
            builder.AddSeqConfiguration();
        })
        .Build()
        .RunAsync();

}
catch (Exception ex)
{
    Log.Logger.Fatal(ex, "Fatal error occurred.");
}
finally
{
    Log.Logger.Information("Application terminated.");
    Log.CloseAndFlush();
}
