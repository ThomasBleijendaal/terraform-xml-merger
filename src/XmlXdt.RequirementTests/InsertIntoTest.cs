﻿using System.Xml;
using Microsoft.Web.XmlTransform;
using Moq;

namespace XmlXdt.RequirementTests;

public class InsertIntoTest
{
    [Test]
    public void Test()
    {
        var targetXml = """
            <policies>
                <inbound>
                    <choose>
                        <when case="100"></when>
                    </choose>
                    <choose id="auth">
                        <when case="1"></when>
                    </choose>
                </inbound>
            </policies>
            """;

        var targetXmlDocument = new XmlDocument();
        targetXmlDocument.LoadXml(targetXml);

        var transformationXml = """
            <policies xmlns:xdt="http://schemas.microsoft.com/XML-Document-Transform">
                <when case="2" xdt:Transform="Insert" xdt:Locator="XPath(/policies/inbound/choose[@id='auth'])"></when>
            </policies>
            """;

        var loggerMock = new Mock<IXmlTransformationLogger>();

        var transform = new XmlTransformation(transformationXml, false, loggerMock.Object);

        var transformationSuccess = transform.Apply(targetXmlDocument);

        Assert.IsTrue(transformationSuccess);

        var resultXml = new XmlDocument();
        resultXml.LoadXml("""
            <policies>
                <inbound>
                    <choose>
                        <when case="100"></when>
                    </choose>
                    <choose id="auth">
                        <when case="1"></when>
                        <when case="2"></when>
                    </choose>
                </inbound>
            </policies>
            """);

        Assert.That(targetXmlDocument.InnerXml, Is.EqualTo(resultXml.InnerXml));
    }
}
