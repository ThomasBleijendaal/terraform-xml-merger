using System.Xml;
using Microsoft.Web.XmlTransform;
using Moq;

namespace XmlXdt.RequirementTests;

public class InsertTest
{
    [Test]
    public void Test()
    {
        var targetXml = """
            <policies>
                <inbound>
                    
                </inbound>
            </policies>
            """;

        var targetXmlDocument = new XmlDocument();
        targetXmlDocument.LoadXml(targetXml);

        var transformationXml = """
            <policies xmlns:xdt="http://schemas.microsoft.com/XML-Document-Transform">
                <inbound>
                    <find-and-replace xdt:Transform="Insert" />
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
                    <find-and-replace />
                </inbound>
            </policies>
            """);

        Assert.That(targetXmlDocument.InnerXml, Is.EqualTo(resultXml.InnerXml));
    }
}
