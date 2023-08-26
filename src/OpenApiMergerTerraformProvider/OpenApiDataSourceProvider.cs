using System.Diagnostics;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using OpenApiMergerTerraformProvider.Extensions;
using TerraformPluginDotNet.ResourceProvider;
using Tfplugin5;

namespace OpenApiMergerTerraformProvider;

public class OpenApiDataSourceProvider : IDataSourceProvider<OpenApiDataResource>
{
    private readonly HttpClient _httpClient;
    private readonly PluginConfiguration _config;

    public OpenApiDataSourceProvider(
        HttpClient httpClient,
        PluginConfigurator config)
    {
        _httpClient = httpClient;
        _config = config.Config;
    }

    public Task<OpenApiDataResource> ReadAsync(OpenApiDataResource request) => throw new NotSupportedException();

    public async Task<OpenApiDataResource> ReadAsync(OpenApiDataResource request, TerraformContext context)
    {
        var rootDocument = new OpenApiDocument
        {
            Info = new()
            {
                Title = request.Title,
                Version = request.Version
            },
            Paths = new(),
            Components = new()
        };

        foreach (var functionApp in request.FunctionApps)
        {
            // TODO: environment values?

            var functionLocation = Path.Combine(_config.RootFolder, functionApp.Path);

            var startInfo = new ProcessStartInfo
            {
                // TODO: configure?
                FileName = "C:\\Program Files\\Microsoft\\Azure Functions Core Tools\\func.exe",
                Arguments = "start",
                WorkingDirectory = functionLocation,

            };

            using var process = Process.Start(startInfo);

            var attempts = 0;
            do
            {
                try
                {
                    await ProcessOpenApiDocumentAsync(rootDocument, functionApp.OpenApiUrl);

                    break;
                }
                catch (Exception ex)
                {
                    await Task.Delay(5000);

                    // TODO: configure?
                    if (++attempts >= 10)
                    {
                        context.AddDiagnostic(new Diagnostic
                        {
                            Summary = $"Failed to start function in '{functionLocation}'.",
                            Severity = Diagnostic.Types.Severity.Error,
                            Detail = $"{ex.Message}\r\n\r\n{ex.StackTrace}"
                        });
                    }
                }
            }
            while (true);

            process?.Kill(entireProcessTree: true);
        }

        foreach (var url in request.OpenApiUrls)
        {
            await ProcessOpenApiDocumentAsync(rootDocument, url);
        }

        var textWriter = new StringWriter();
        var writer = new OpenApiJsonWriter(textWriter);

        rootDocument.SerializeAsV3(writer);

        return request with
        {
            OpenApiJson = textWriter.ToString()
        };
    }

    private async Task ProcessOpenApiDocumentAsync(OpenApiDocument rootDocument, string? url)
    {
        // TODO: should support local files

        var jsonStream = await _httpClient.GetStreamAsync(url);

        var reader = new OpenApiStreamReader();
        var document = reader.Read(jsonStream, out var diagnostic);

        rootDocument.Servers.AddRange(document.Servers);
        rootDocument.Paths.AddOrSetRange(document.Paths);
        rootDocument.Components.Schemas.AddOrSetRange(document.Components.Schemas);
        rootDocument.Components.SecuritySchemes.AddOrSetRange(document.Components.SecuritySchemes);
    }
}
