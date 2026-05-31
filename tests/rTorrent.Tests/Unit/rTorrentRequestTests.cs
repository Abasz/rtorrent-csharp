using rTorrentLib;
using rTorrentLib.Model;
using Xunit;

namespace rTorrent.Tests.Unit;

public class rTorrentRequestTests
{
    [Fact]
    public void GivenSingleCommand_ProducesMethodCallXml()
    {
        var commands = new List<TorrentCommand> { new("d.name", "abc123") };
        var sut = new rTorrentRequest(commands);

        Assert.Contains("<methodName>system.multicall</methodName>", sut.CommandString);
        Assert.Contains("<string>d.name</string>", sut.CommandString);
    }

    [Fact]
    public void GivenMultipleCommands_WrapsInSystemMulticall()
    {
        var commands = new List<TorrentCommand>
        {
            new("d.name", "abc123"),
            new("d.size_bytes", "abc123"),
        };
        var sut = new rTorrentRequest(commands);

        Assert.Contains("system.multicall", sut.CommandString);
        Assert.Contains("<string>d.name</string>", sut.CommandString);
        Assert.Contains("<string>d.size_bytes</string>", sut.CommandString);
    }

    [Fact]
    public void GivenEmptyCommandList_Throws()
    {
        Assert.Throws<NullReferenceException>(() =>
            new rTorrentRequest(new List<TorrentCommand>()));
    }

    [Fact]
    public void GivenCommandWithIntParam_SerializesI4Type()
    {
        var commands = new List<TorrentCommand>
        {
            new("throttle.up", new List<TorrentParams> { new TorrentParams(42) })
        };
        var sut = new rTorrentRequest(commands);

        Assert.Contains("<i4>42</i4>", sut.CommandString);
    }
}
