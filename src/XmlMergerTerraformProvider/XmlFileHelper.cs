using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace XmlMergerTerraformProvider;

public static partial class XmlFileHelper
{
    private static readonly Regex ParamRegex = CreateParamRegex();
    private static readonly Regex NamedValueRegex = CreateNamedValueRegex();

    public static IEnumerable<KeyValuePair<string, Type>> ResolveParametersFromXmlFile(string policyFile)
    {
        var xmlDocument = new XmlDocument();

        xmlDocument.Load(policyFile);

        var fragment = new Dictionary<string, Type>();

        var matchGroups = ParamRegex
            .Matches(xmlDocument.InnerXml)
            .OfType<Match>()
            .Where(x => x.Groups.Count == 3)
            .Select(x => x.Groups);

        foreach (var matchGroup in matchGroups)
        {
            var type = matchGroup[2].Value switch
            {
                "bool" => typeof(bool),
                "int" => typeof(int),
                "long" => typeof(long),
                _ => typeof(string)
            };

            fragment.Add(matchGroup[1].Value.ToLower(), type);
        }

        return fragment;
    }

    public static string ApplyParametersToXmlFile(string policyFile, IEnumerable<KeyValuePair<string, object>> parameters)
    {
        foreach (var parameter in parameters)
        {
            var regex = new Regex($"%{parameter.Key.ToUpper()}:([a-z]+)%");

            policyFile = regex.Replace(policyFile, parameter.Value.ToString() ?? "");
        }

        return policyFile;
    }

    public static string GetFormattedXml(this XmlDocument doc)
    {
        var sb = new StringBuilder();
        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace
        };

        using var writer = XmlWriter.Create(sb, settings);

        doc.Save(writer);

        return sb.ToString();
    }

    public static List<string> ResolveNamedValuesFromXmlFile(string policyFile)
    {
        return NamedValueRegex
            .Matches(policyFile)
            .Select(x => x.Groups.Values.Skip(1).FirstOrDefault()?.Value)
            .OfType<string>()
            .ToList();
    }

    [GeneratedRegex("%([A-Z_]+):([a-z]+)%")]
    private static partial Regex CreateParamRegex();

    [GeneratedRegex(@"{{([A-Za-z0-9\.\-_]+)}}")]
    private static partial Regex CreateNamedValueRegex();
}
