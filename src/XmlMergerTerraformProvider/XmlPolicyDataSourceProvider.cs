using System.Xml;
using Microsoft.Web.XmlTransform;
using TerraformPluginDotNet.ResourceProvider;
using Tfplugin5;

namespace XmlMergerTerraformProvider;

public class XmlPolicyDataSourceProvider : IDataSourceProvider<XmlPolicy>
{
    private readonly PluginConfigurator _config;

    public XmlPolicyDataSourceProvider(
        PluginConfigurator config)
    {
        _config = config;
    }

    public Task<XmlPolicy> ReadAsync(XmlPolicy request) => throw new NotSupportedException();

    public Task<XmlPolicy> ReadAsync(XmlPolicy request, TerraformContext context)
    {
        var config = _config.Config;

        var basePolicyXml = new XmlDocument();
        try
        {
            if (request.BaseXml != null)
            {
                basePolicyXml.LoadXml(request.BaseXml);
            }
            else
            {
                basePolicyXml.Load(config.BasePolicy);
            }
        }
        catch (Exception ex)
        {
            context.AddDiagnostic(new Diagnostic
            {
                Severity = Diagnostic.Types.Severity.Error,
                Summary = "Failed to load base policy xml",
                Detail = $"Tried to load\r\n\r\n{request.BaseXml ?? config.BasePolicy}\r\n\r\n{ex.Message}\r\n{ex.StackTrace}"
            });
        }

        foreach (var fragment in request.Fragments.Where(x => x.Value != null))
        {
            var xmlFileName = Path.Combine(config.PolicyFolder, $"{fragment.Key}.xml");

            var xml = XmlFileHelper.ApplyParametersToXmlFile(
                File.ReadAllText(xmlFileName),
                fragment.Value);

            var transformFragment = new XmlTransformation(xml, false, null);

            try
            {
                if (!transformFragment.Apply(basePolicyXml))
                {
                    context.AddDiagnostic(new Diagnostic
                    {
                        Severity = Diagnostic.Types.Severity.Error,
                        Summary = $"Failed to apply {fragment.Key} fragment to xml document",
                        Detail = $"Applied to \r\n\r\n{basePolicyXml.GetFormattedXml()}"
                    });
                }
            }
            catch (Exception ex)
            {
                context.AddDiagnostic(new Diagnostic
                {
                    Severity = Diagnostic.Types.Severity.Error,
                    Summary = $"Applying {fragment.Key} fragment to xml document threw an exception",
                    Detail = $"Applied to \r\n\r\n{basePolicyXml.GetFormattedXml()}\r\n\r\n{ex.Message}\r\n{ex.StackTrace}"
                });
            }
        }

        var output = request with
        {
            Xml = basePolicyXml.GetFormattedXml()
        };

        return Task.FromResult(output);
    }
}
