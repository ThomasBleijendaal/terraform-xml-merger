using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using OpenApiMerger.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

var app = builder.Build();

app.Map("/json", async ([FromQuery] string[] url, [FromServices] HttpClient httpClient, CancellationToken cancellationToken) =>
{
    var urls = url.Select(url => Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri : null).OfType<Uri>().ToArray();

    var rootDocument = new OpenApiDocument
    {
        Info = new()
        {
            Title = "MergedOpenApi",
            Version = "v1"
        },
        Paths = new(),
        Components = new()
    };

    var documents = await Task.WhenAll(urls.Select(async uri =>
    {
        using var jsonStream = await httpClient.GetStreamAsync(uri.ToString(), cancellationToken);

        var reader = new OpenApiStreamReader();
        var schema = reader.Read(jsonStream, out var diagnostic);

        return schema;
    }));

    foreach (var document in documents)
    {
        rootDocument.Servers.AddRange(document.Servers);

        rootDocument.Paths.AddOrSetRange(document.Paths);

        rootDocument.Components.Schemas.AddOrSetRange(document.Components.Schemas);

        rootDocument.Components.SecuritySchemes.AddOrSetRange(document.Components.SecuritySchemes);
    }

    var textWriter = new StringWriter();
    var writer = new OpenApiJsonWriter(textWriter);

    rootDocument.SerializeAsV3(writer);

    return Results.Text(textWriter.ToString(), "application/json");
});

app.Run();
