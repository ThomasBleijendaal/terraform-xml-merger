using System.Collections.Immutable;
using Google.Protobuf;
using Microsoft.OpenApi.Readers;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Schemas.Types;
using Tfplugin5;

namespace OpenApiMergerTerraformProvider;

public class OpenApiSchemaProvider : IDataSourceSchemaProvider
{
    private readonly HttpClient _httpClient;
    private readonly ITerraformTypeBuilder _terraformTypeBuilder;

    public OpenApiSchemaProvider(
        HttpClient httpClient,
        ITerraformTypeBuilder terraformTypeBuilder)
    {
        _httpClient = httpClient;
        _terraformTypeBuilder = terraformTypeBuilder;
    }

    public IEnumerable<DataSourceRegistration> GetSchemas()
    {
        var url = Environment.GetEnvironmentVariable("TF_OPENAPI_URL");

        var response = _httpClient.Send(new HttpRequestMessage(HttpMethod.Get, url));
        var jsonStream = response.Content.ReadAsStream();

        var reader = new OpenApiStreamReader();
        var openApiDocument = reader.Read(jsonStream, out var diagnostic);

        if (openApiDocument != null)
        {
            var operations = new Dictionary<string, TerraformType>();
            var terraformStringType = _terraformTypeBuilder.GetTerraformType(typeof(string));

            foreach (var path in openApiDocument.Paths)
            {
                foreach (var pathOperation in path.Value.Operations)
                {
                    operations.Add(pathOperation.Value.OperationId, terraformStringType);
                }
            }

            var operationsType = new TerraformType.TfObject(operations.ToImmutableDictionary(), operations.Keys.ToImmutableHashSet());

            // TODO: use schema builder to build the base

            var schema = new Schema
            {
                Version = 1,
                Block = new Schema.Types.Block
                {
                    Attributes =
                    {
                        new Schema.Types.Attribute
                        {
                            Computed = true,
                            Description = "OpenApi JSON",
                            DescriptionKind = StringKind.Plain,
                            Name = "open_api_json",
                            Type = ByteString.CopyFromUtf8(terraformStringType.ToJson())
                        },
                        new Schema.Types.Attribute
                        {
                            Computed = true,
                            Description = "Operations",
                            DescriptionKind = StringKind.Plain,
                            Name = "operations",
                            Type = ByteString.CopyFromUtf8(operationsType.ToJson())
                        }
                    }
                }
            };

            yield return new DataSourceRegistration("openapimerger", typeof(OpenApiDataResource), schema);
        }
    }
}
