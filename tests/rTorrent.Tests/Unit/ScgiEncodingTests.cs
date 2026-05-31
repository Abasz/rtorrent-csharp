using System.Text;
using rTorrentLib;
using Xunit;

namespace rTorrent.Tests.Unit;

public class ScgiEncodingTests
{
    private const string SampleXml =
        "<?xml version='1.0' encoding='UTF-8'?><methodCall><methodName>system.listMethods</methodName></methodCall>";

    [Fact]
    public void Encode_HeaderLengthPrefixIsAccurate()
    {
        var result = rTorrentClient.BuildScgiRequest(SampleXml);
        var text = Encoding.UTF8.GetString(result);

        // Format: "{headerLen}:{header},{body}"
        var colonIndex = text.IndexOf(':');
        Assert.True(colonIndex > 0, "Netstring colon not found");

        var declaredHeaderLen = int.Parse(text[..colonIndex]);
        var headerStart = colonIndex + 1;
        var commaIndex = headerStart + declaredHeaderLen;

        Assert.True(commaIndex < text.Length, "Declared header length exceeds buffer");
        Assert.Equal(',', text[commaIndex]);
        Assert.Equal(declaredHeaderLen, commaIndex - headerStart);
    }

    [Fact]
    public void Encode_ContentLengthMatchesXmlBody()
    {
        var result = rTorrentClient.BuildScgiRequest(SampleXml);
        var text = Encoding.UTF8.GetString(result);

        var bodyBytes = Encoding.UTF8.GetBytes(SampleXml);
        Assert.Contains($"CONTENT_LENGTH\0{bodyBytes.Length}\0", text);
    }

    [Fact]
    public void Encode_NoUntrustedConnectionHeader()
    {
        var result = rTorrentClient.BuildScgiRequest(SampleXml);
        var text = Encoding.UTF8.GetString(result);

        Assert.DoesNotContain("UNTRUSTED_CONNECTION", text);
    }
}
