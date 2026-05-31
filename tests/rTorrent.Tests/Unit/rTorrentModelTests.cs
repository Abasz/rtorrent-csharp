using rTorrentLib.Model;
using Xunit;

namespace rTorrent.Tests.Unit;

public class rTorrentModelTests
{
    // TorrentParams type inference

    [Fact]
    public void TorrentParams_Integer_TypeIsI4()
    {
        var sut = new TorrentParams(1);
        Assert.Equal("i4", sut.Type);
    }

    [Fact]
    public void TorrentParams_Long_TypeIsI4()
    {
        var sut = new TorrentParams(1L);
        Assert.Equal("i4", sut.Type);
    }

    [Fact]
    public void TorrentParams_Double_TypeIsI8()
    {
        var sut = new TorrentParams(1.5);
        Assert.Equal("i8", sut.Type);
    }

    [Fact]
    public void TorrentParams_Float_TypeIsI8()
    {
        var sut = new TorrentParams(1.5f);
        Assert.Equal("i8", sut.Type);
    }

    [Fact]
    public void TorrentParams_String_TypeIsString()
    {
        var sut = new TorrentParams("hello");
        Assert.Equal("string", sut.Type);
    }

    [Fact]
    public void TorrentParams_ExplicitType_Overrides()
    {
        var sut = new TorrentParams("raw-value", "base64");
        Assert.Equal("base64", sut.Type);
    }

    // TorrentDMultiCallCommand

    [Fact]
    public void DMultiCallCommand_InjectsEmptyStringAndView()
    {
        var sut = new TorrentDMultiCallCommand("active", "d.name=");

        Assert.Equal("d.multicall2", sut.Command);
        // Parameters: ["", "active", "d.name="]
        Assert.Equal(3, sut.Parameters.Count);
        Assert.Equal("", sut.Parameters[0].Parameter);
        Assert.Equal("active", sut.Parameters[1].Parameter);
    }

    [Fact]
    public void DMultiCallCommand_DefaultView_IsMain()
    {
        var sut = new TorrentDMultiCallCommand("main", "d.name=");
        Assert.Equal("main", sut.View);
    }

    // TorrentFMultiCallCommand

    [Fact]
    public void FMultiCallCommand_InjectsHashAndEmptyString()
    {
        var hash = "AABBCCDDEEFF00112233445566778899AABBCCDD";
        var sut = new TorrentFMultiCallCommand(hash, "f.path=");

        Assert.Equal("f.multicall", sut.Command);
        // Parameters: [hash, "", "f.path="]
        Assert.Equal(3, sut.Parameters.Count);
        Assert.Equal(hash, sut.Parameters[0].Parameter);
        Assert.Equal("", sut.Parameters[1].Parameter);
    }
}
