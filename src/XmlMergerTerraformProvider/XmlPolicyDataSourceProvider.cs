using System.Xml;
using Microsoft.Web.XmlTransform;
using TerraformPluginDotNet.ResourceProvider;
using Tfplugin5;

namespace XmlMergerTerraformProvider;

public class XmlPolicyDataSourceProvider : IDataSourceProvider<XmlPolicyDataResource>
{
    private readonly PluginConfigurator _config;

    public XmlPolicyDataSourceProvider(
        PluginConfigurator config)
    {
        _config = config;
    }

    public Task<XmlPolicyDataResource> ReadAsync(XmlPolicyDataResource request) => throw new NotSupportedException();

    public Task<XmlPolicyDataResource> ReadAsync(XmlPolicyDataResource request, TerraformContext context)
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

        foreach (var fragmentSet in request.Fragments)
        {
            foreach (var fragment in fragmentSet.Where(x => x.Value != null))
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
                            Detail = $"Applied to\r\n\r\n{basePolicyXml.GetFormattedXml()}"
                        });
                    }
                }
                catch (Exception ex)
                {
                    context.AddDiagnostic(new Diagnostic
                    {
                        Severity = Diagnostic.Types.Severity.Error,
                        Summary = $"Applying {fragment.Key} fragment to xml document threw an exception",
                        Detail = $"Applied to\r\n\r\n{basePolicyXml.GetFormattedXml()}\r\n\r\n{ex.Message}\r\n{ex.StackTrace}"
                    });
                }
            }
        }

        var policyXml = basePolicyXml.GetFormattedXml();

        var namedValues = XmlFileHelper.ResolveNamedValuesFromXmlFile(policyXml);

        var compiledNamedValues = namedValues
            .Select(x => x.Split("__")[0])
            .ToDictionary(x => x, x => $"{x}__{request.PolicyName}");

        foreach (var kv in compiledNamedValues)
        {
            policyXml = policyXml.Replace(
                $$$"""{{{{{kv.Key}}}}}""",
                $$$"""{{{{{kv.Value}}}}}""");
        }

        var output = request with
        {
            UsedNamedValues = compiledNamedValues,
            Xml = policyXml
        };

        return Task.FromResult(output);
    }
}
