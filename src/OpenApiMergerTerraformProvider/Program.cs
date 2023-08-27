using Core;
using OpenApiMergerTerraformProvider;
using Serilog;
using TerraformPluginDotNet;
using TerraformPluginDotNet.ResourceProvider;

try
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(new ConfigurationBuilder().AddSeqConfiguration().Build())
        .CreateBootstrapLogger();

    await TerraformPluginHost.CreateHostBuilder(args, "thomas-ict.nl/azure/openapimerger")
        .ConfigureResourceRegistry((services, registry) =>
        {
            services.AddHttpClient();
            services.AddSingleton<PluginConfigurator>();
            services.AddTerraformProviderConfigurator<PluginConfiguration, PluginConfigurator>();
            services.AddTransient<IDataSourceProvider<OpenApiDataResource>, OpenApiDataSourceProvider>();
            registry.RegisterDataSource<OpenApiDataResource>("openapimerger");
        }, 5344)
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
