using System.Xml.Linq;
using rTorrentLib;
using Xunit;

namespace rTorrent.Tests.Unit;

public class rTorrentResponseTests
{
    // A minimal valid SCGI response for a successful multicall that returns one string value.
    private static string ValidXmlResponse(string innerXml) =>
        $"Status: 200 OK\nContent-Type: text/xml\nContent-Length: {innerXml.Length}\n\n{innerXml}";

    private static readonly string SimpleBodyXml =
        "<?xml version='1.0' encoding='UTF-8'?>" +
        "<methodResponse><params><param><value><array><data>" +
        "<value><array><data><value><string>hello</string></value></data></array></value>" +
        "</data></array></value></param></params></methodResponse>";

    private static readonly string FaultBodyXml =
        "<?xml version='1.0' encoding='UTF-8'?>" +
        "<methodResponse><params><param><value><array><data>" +
        "<value><struct>" +
        "<member><name>faultCode</name><value><i4>-500</i4></value></member>" +
        "<member><name>faultString</name><value><string>Method not found</string></value></member>" +
        "</struct></value>" +
        "</data></array></value></param></params></methodResponse>";

    [Fact]
    public void GivenValidXmlResponse_Status200()
    {
        var sut = new rTorrentResponse<string>(ValidXmlResponse(SimpleBodyXml), _ => "ok");

        Assert.Equal(200, sut.Status);
    }

    [Fact]
    public void GivenValidXmlResponse_ContentTypeParsed()
    {
        var sut = new rTorrentResponse<string>(ValidXmlResponse(SimpleBodyXml), _ => "ok");

        Assert.Equal("text/xml", sut.ContentType);
    }

    [Fact]
    public void GivenValidXmlResponse_ContentLengthParsed()
    {
        var sut = new rTorrentResponse<string>(ValidXmlResponse(SimpleBodyXml), _ => "ok");

        Assert.Equal(SimpleBodyXml.Length, sut.ContentLength);
    }

    [Fact]
    public void GivenValidXmlResponse_BodyPassedToParser()
    {
        IEnumerable<XElement>? capturedElements = null;

        var sut = new rTorrentResponse<string>(ValidXmlResponse(SimpleBodyXml), elements =>
        {
            capturedElements = elements;
            return "parsed";
        });

        Assert.NotNull(capturedElements);
        Assert.Equal("parsed", sut.Body);
    }

    [Fact]
    public void GivenEmptyResponse_Returns503()
    {
        var sut = new rTorrentResponse<string>(string.Empty, _ => "ok");

        Assert.Equal(503, sut.Status);
        Assert.NotNull(sut.ErrorMessage);
    }

    [Fact]
    public void GivenTooFewLines_Returns503()
    {
        var sut = new rTorrentResponse<string>("Status: 200 OK", _ => "ok");

        Assert.Equal(503, sut.Status);
        Assert.NotNull(sut.ErrorMessage);
    }

    [Fact]
    public void GivenXmlRpcFaultResponse_ParsesFaultCodeAndMessage()
    {
        var sut = new rTorrentResponse<string>(ValidXmlResponse(FaultBodyXml), _ => "ok");

        Assert.Equal(500, sut.Status);
        Assert.Equal("Method not found", sut.ErrorMessage);
    }
}
