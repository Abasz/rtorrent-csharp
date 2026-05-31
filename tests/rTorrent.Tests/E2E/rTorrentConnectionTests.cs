using System.Net.Sockets;
using System.Xml.Linq;
using rTorrentLib.Model;
using rTorrentLib.Services;
using Xunit;

namespace rTorrent.Tests.E2E;

/// <summary>
/// End-to-end tests that connect to a live rTorrent instance via SCGI.
/// All commands are read-only — no writes are performed.
///
/// Configure the target via environment variables:
///   RTORRENT_HOST  (default: 127.0.0.1)
///   RTORRENT_PORT  (default: 5000)
///
/// Run only these tests with:
///   dotnet test --filter "Category=E2E"
/// </summary>
[Trait("Category", "E2E")]
public class rTorrentConnectionTests
{
    private readonly string _host;
    private readonly int _port;

    public rTorrentConnectionTests()
    {
        _host = Environment.GetEnvironmentVariable("RTORRENT_HOST") ?? "127.0.0.1";
        _port = int.TryParse(Environment.GetEnvironmentVariable("RTORRENT_PORT"), out var p) ? p : 5000;
    }

    private void SkipIfUnavailable()
    {
        try
        {
            using var probe = new TcpClient();
            probe.Connect(_host, _port);
        }
        catch
        {
            Assert.Skip($"rTorrent not reachable at {_host}:{_port} — set RTORRENT_HOST/RTORRENT_PORT or start rTorrent.");
        }
    }

    private rTorrentService CreateService() => new(_host, _port);

    [Fact]
    public async Task CanConnect_SystemListMethods()
    {
        SkipIfUnavailable();

        var service = CreateService();
        var commands = new List<TorrentCommand> { new("system.listMethods") };

        var response = await service.SendTorrentQueryAsync<IEnumerable<XElement>>(
            commands,
            elements => elements);

        Assert.Equal(200, response.Status);
        Assert.NotNull(response.Body);
        Assert.True(response.Body.Any(), "system.listMethods returned empty list");
    }

    [Fact]
    public async Task CanConnect_GetTorrentList()
    {
        SkipIfUnavailable();

        var service = CreateService();
        var commands = new List<TorrentCommand>
        {
            new TorrentDMultiCallCommand("main", "d.hash=", "d.name=")
        };

        var response = await service.SendTorrentQueryAsync(commands);

        Assert.Equal(200, response.Status);
        Assert.Null(response.ErrorMessage);
    }

    [Fact]
    public async Task CanConnect_MulticallRoundTrip()
    {
        SkipIfUnavailable();

        var service = CreateService();
        var commands = new List<TorrentCommand>
        {
            new("system.listMethods"),
            new("system.listMethods"),
        };

        var response = await service.SendTorrentQueryAsync(commands);

        Assert.Equal(200, response.Status);
        Assert.NotNull(response.Body);
    }

    [Fact]
    public async Task InvalidCommand_ReturnsFault()
    {
        SkipIfUnavailable();

        var service = CreateService();
        var commands = new List<TorrentCommand> { new("nonexistent.method.xyz") };

        var response = await service.SendTorrentQueryAsync(commands);

        Assert.NotEqual(200, response.Status);
        Assert.NotNull(response.ErrorMessage);
    }

    [Fact]
    public async Task CanConnect_SystemGetCapabilities()
    {
        SkipIfUnavailable();

        var service = CreateService();
        var commands = new List<TorrentCommand> { new("system.getCapabilities") };

        var response = await service.SendTorrentQueryAsync(commands);

        // getCapabilities may not be available on all rTorrent builds;
        // we accept 200 or a fault — either proves the SCGI round-trip works.
        Assert.True(response.Status == 200 || response.ErrorMessage != null,
            "Expected either a 200 response or a fault, but got neither.");
    }
}
