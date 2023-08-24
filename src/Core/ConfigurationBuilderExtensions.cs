using Microsoft.Extensions.Configuration;

namespace Core;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddSeqConfiguration(this IConfigurationBuilder builder)
        => builder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Serilog:Using:0", "Serilog.Sinks.Seq" },
            { "Serilog:Enrich:0", "FromLogContext" },
            { "Serilog:WriteTo:0:Name", "Seq" },
            { "Serilog:WriteTo:0:Args:serverUrl", "http://localhost:5341" },
            { "Serilog:WriteTo:0:Args:apiKey", "-" },
            { "Serilog:MinimumLevel:Default", "Information" },
            { "Serilog:MinimumLevel:Override:Grpc", "Warning" },
            { "Serilog:MinimumLevel:Override:Microsoft", "Information" },
            { "Serilog:MinimumLevel:Override:Microsoft.AspNetCore.Routing.EndpointMiddleware", "Error" },
            { "Serilog:MinimumLevel:Override:Microsoft.AspNetCore.Server.Kestrel", "Error" },
            { "Serilog:Filter:0:Name", "ByExcluding" },
            { "Serilog:Filter:0:Args:expression", "RequestPath like '%plugin.%'" },
            { "Serilog:Filter:1:Name", "ByExcluding" },
            { "Serilog:Filter:1:Args:expression", "RequestPath like '%tfplugin5.Provider/GetSchema'" },
            { "Serilog:Filter:2:Name", "ByExcluding" },
            { "Serilog:Filter:2:Args:expression", "RequestPath like '%tfplugin5.Provider/ValidateResourceTypeConfig'" },
            { "Serilog:Filter:3:Name", "ByExcluding" },
            { "Serilog:Filter:3:Args:expression", "RequestPath like '%tfplugin5.Provider/Configure'" },
            { "Serilog:Filter:4:Name", "ByExcluding" },
            { "Serilog:Filter:4:Args:expression", "RequestPath like '%tfplugin5.Provider/PrepareProviderConfig'" },
        });
}
