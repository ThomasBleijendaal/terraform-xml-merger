using System.Xml;
using Microsoft.Web.XmlTransform;
using Moq;

namespace XmlXdt.RequirementTests;

public class InsertFirstTest
{
    [Test]
    public void Test()
    {
        var targetXml = """
            <policies>
                <inbound>
                    <element-b />
                    <element-c />
                </inbound>
            </policies>
            """;

        var targetXmlDocument = new XmlDocument();
        targetXmlDocument.LoadXml(targetXml);

        var transformationXml = """
            <policies xmlns:xdt="http://schemas.microsoft.com/XML-Document-Transform">
                <inbound>
                    <element-a xdt:Transform="InsertBefore(/policies/inbound/*[1])" />
                </inbound>
            </policies>
            """;

        var transform = new XmlTransformation(transformationXml, false, Mock.Of<IXmlTransformationLogger>());

        var transformationSuccess = transform.Apply(targetXmlDocument);

        Assert.IsTrue(transformationSuccess);

        var resultXml = new XmlDocument();
        resultXml.LoadXml("""
            <policies>
                <inbound>
                    <element-a />
                    <element-b />
                    <element-c />
                </inbound>
            </policies>
            """);

        Assert.That(targetXmlDocument.InnerXml, Is.EqualTo(resultXml.InnerXml));
    }
}
