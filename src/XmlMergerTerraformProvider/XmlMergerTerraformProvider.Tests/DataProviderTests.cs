using Microsoft.Extensions.DependencyInjection;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Testing;

namespace XmlMergerTerraformProvider.Tests;

[TestFixture(Category = "Functional", Explicit = true)]
public class DataProviderTests
{
    private const string ProviderName = "xmlmerger";

    private TerraformTestHost _host;

    [OneTimeSetUp]
    public void Setup()
    {
        _host = new TerraformTestHost(Environment.GetEnvironmentVariable("TF_PLUGIN_DOTNET_TEST_TF_BIN"));
        _host.Start($"thomas-ict.nl/azure/{ProviderName}", Configure);
    }

    [OneTimeTearDown]
    public async Task TearDownAsync()
    {
        await _host.DisposeAsync();
    }

    private void Configure(IServiceCollection services, IResourceRegistryContext registryContext)
    {
        services.AddSingleton<IDataSourceProvider<XmlPolicy>, XmlPolicyDataSourceProvider>();
        services.AddTransient<IDataSourceSchemaProvider, XmlFileSchemaProvider>();
    }

    [Test]
    public async Task TestPlanTestAsync()
    {
        using var terraform = await _host.CreateTerraformTestInstanceAsync(ProviderName);

        var resourcePath = Path.Combine(terraform.WorkDir, "file.tf");

        await File.WriteAllTextAsync(resourcePath, """
            data "xmlmerger_test" "test" {

            }

            output "test" {
              value = data.xmlmerger_test.test.policy_name
            }

            """);

        var plan = await terraform.PlanWithOutputAsync();

        Assert.That(plan.PlannedValues.Outputs["test"].Value, Is.EqualTo("Abc"));
    }
}
