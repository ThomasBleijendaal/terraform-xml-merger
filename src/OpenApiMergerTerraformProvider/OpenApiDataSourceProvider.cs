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
    private readonly Dictionary<string, string> _emptyEnv = new();

    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenApiDataSourceProvider> _logger;
    private readonly PluginConfiguration _config;

    public OpenApiDataSourceProvider(
        HttpClient httpClient,
        PluginConfigurator config,
        ILogger<OpenApiDataSourceProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
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

        var cacheRootLocation = Path.Combine(Directory.GetCurrentDirectory(), ".terraform", "cache");

        Directory.CreateDirectory(cacheRootLocation);

        // TODO: run in parallel with separate ports

        if (request.FunctionApps != null)
        {
            foreach (var functionApp in request.FunctionApps)
            {
                var cacheFile = Path.Combine(cacheRootLocation, $"{functionApp.Path}.json");

                if (File.Exists(cacheFile) && File.GetLastWriteTime(cacheFile) > DateTime.Now.AddHours(-1))
                {
                    _logger.LogInformation("Loading swagger from {cacheFile}", cacheFile);

                    ProcessOpenApiDocument(rootDocument, cacheFile);

                    continue;
                }

                _logger.LogInformation("Starting {functionAppPath}", functionApp.Path);

                var functionLocation = Path.Combine(_config.RootFolder, functionApp.Path);

                var localSettings = Path.Combine(functionLocation, "local.settings.json");
                var localSettingsExample = Path.Combine(functionLocation, !string.IsNullOrWhiteSpace(_config.FunctionSettingsExampleJson)
                    ? _config.FunctionSettingsExampleJson
                    : "local.settings.example.json");

                if (!File.Exists(localSettings) && File.Exists(localSettingsExample))
                {
                    _logger.LogInformation("Copying {localSettingsExample} to {localSettings}", localSettingsExample, localSettings);
                    File.Copy(localSettingsExample, localSettings);
                }

                var startInfo = new ProcessStartInfo
                {
                    Arguments = "start",
                    FileName = !string.IsNullOrWhiteSpace(_config.FunctionCoreTools)
                        ? _config.FunctionCoreTools
                        : "C:\\Program Files\\Microsoft\\Azure Functions Core Tools\\func.exe",
                    WorkingDirectory = functionLocation
                };

                var variables = (functionApp.EnvVariables ?? _emptyEnv)
                    .Union(_config.EnvVariables ?? _emptyEnv)
                    .DistinctBy(x => x.Key)
                    .ToArray();

                startInfo.EnvironmentVariables.AddRange(variables);

                using var process = Process.Start(startInfo);

                var attempts = 0;
                do
                {
                    try
                    {
                        _logger.LogInformation("Trying {functionAppOpenApiUrl}", functionApp.OpenApiUrl);

                        var json = await _httpClient.GetStringAsync(functionApp.OpenApiUrl);

                        File.WriteAllText(cacheFile, json);

                        ProcessOpenApiDocument(rootDocument, cacheFile);

                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex, "Failed to access {functionAppOpenApiUrl}", functionApp.OpenApiUrl);

                        await Task.Delay(_config.RetryDelay ?? 5000);

                        if (++attempts >= (_config.RetryCount ?? 10))
                        {
                            _logger.LogError(ex, "Completely failed to access {functionAppOpenApiUrl}", functionApp.OpenApiUrl);

                            context.AddDiagnostic(new Diagnostic
                            {
                                Summary = $"Failed to start function in '{functionLocation}'.",
                                Severity = Diagnostic.Types.Severity.Error,
                                Detail = $"{ex.Message}\r\n\r\n{ex.StackTrace}"
                            });

                            break;
                        }
                    }
                }
                while (true);

                process?.Kill(entireProcessTree: true);
            }
        }

        if (request.OpenApiUrls != null)
        {
            foreach (var url in request.OpenApiUrls)
            {
                try
                {
                    _logger.LogInformation("Trying {openApiUrl}", url);

                    await ProcessOpenApiDocumentAsync(rootDocument, url);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to access {openApiUrl}", url);

                    context.AddDiagnostic(new Diagnostic
                    {
                        Summary = $"Failed to load '{url}'.",
                        Severity = Diagnostic.Types.Severity.Error,
                        Detail = $"{ex.Message}\r\n\r\n{ex.StackTrace}"
                    });
                }
            }
        }

        if (request.OpenApiFiles != null)
        {
            foreach (var file in request.OpenApiFiles)
            {
                try
                {
                    _logger.LogInformation("Trying {openApiFile}", file);

                    ProcessOpenApiDocument(rootDocument, file);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to access {openApiFile}", file);

                    context.AddDiagnostic(new Diagnostic
                    {
                        Summary = $"Failed to load '{file}'.",
                        Severity = Diagnostic.Types.Severity.Error,
                        Detail = $"{ex.Message}\r\n\r\n{ex.StackTrace}"
                    });
                }
            }
        }

        var textWriter = new StringWriter();
        var writer = new OpenApiJsonWriter(textWriter);

        rootDocument.SerializeAsV3(writer);

        return request with
        {
            OpenApiJson = textWriter.ToString()
        };
    }

    private async Task ProcessOpenApiDocumentAsync(OpenApiDocument rootDocument, string url)
    {
        using var jsonStream = await _httpClient.GetStreamAsync(url);

        MergeOpenApiDocument(rootDocument, jsonStream);
    }

    private static void ProcessOpenApiDocument(OpenApiDocument rootDocument, string file)
    {
        using var fileStream = File.OpenRead(file);

        MergeOpenApiDocument(rootDocument, fileStream);
    }

    private static void MergeOpenApiDocument(OpenApiDocument rootDocument, Stream openApiStream)
    {
        var reader = new OpenApiStreamReader();
        var document = reader.Read(openApiStream, out _);

        foreach (var server in document.Servers)
        {
            if (!rootDocument.Servers.Any(x => x.Url == server.Url))
            {
                rootDocument.Servers.Add(server);
            }
        }
        rootDocument.Paths.AddOrSetRange(document.Paths);
        rootDocument.Components.Schemas.AddOrSetRange(document.Components.Schemas);
        rootDocument.Components.SecuritySchemes.AddOrSetRange(document.Components.SecuritySchemes);
    }
}
