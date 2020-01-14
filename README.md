# rTorrent - C# Interface

Interface written in C# for communicating with the rTorrent client via its XMLRPC interface. Commands may be sent thorough C# objects and responses may be parsed from an IEnumerable<XElement> through a parser delegate (alternatively the unparsed IEnumerable<XElement> may be used).

Its important that the interface connects directly to rTorrent's XMLRPC port (i.e. no need to set up Apache/Nginx scgi port)

Parsing the command objects to the appropriate XMLRPC format is taken care of automatically.

In the [rTorrent Handbook](https://rtorrent-docs.readthedocs.io/en/latest/) one may find the community maintained comprehensive manual and user guide for the rTorrent XMLRPC interface with detailed command reference.


## General usage

There is a service wrapper that maybe used with dependency injection e.g. like below:

```cs
services.AddTransient<IrTorrentService, rTorrentService>();
```
or
```cs
services.AddTransient<IrTorrentService>(sp => new rTorrentService("127.0.0.1", 5050));
```
The above takes care of creating an rTorrentService instance and can be injected in other services. The parameterless constructor creates the instance on each request with a default ipAddress of 127.0.0.1 and a port of 5000.

Or a new instance maybe created directly:

```cs
new rTorrentService(string ipAddress = "127.0.0.1", int port = 5000);
```

The commands may be sent with the SendTorrentQueryAsync method that takes a list of torrent commands and an optional parser function as constructor parameters. The method returns an rTorrentResponse object.

```cs
rTorrentResponse SendTorrentQueryAsync<T>(List<TorrentCommand> commands, Func<IEnumerable<XElement>, T> parser);
```

Getting the names and hashes of all torrents as a multicall and parsing it to an object:

```cs
var dMulticall = new List<TorrentCommand>()
    {
        new TorrentDMultiCallCommand(
        "main",
        "name=",
        "hash="
        )
    };

var response = await _torrentService.SendTorrentQueryAsync(dMulticall, responseXml =>
    responseXml
    .ElementAt(0)
    .Descendants("data")
    .ElementAt(0)
    .Descendants("data")
    .Select(item =>
    new TorrentDetails {
        Name = item.ElementAt(0).Value,
        Hash = item.ElementAt(1).Value
    }));
```

#### Command types

- General commands
```cs
new TorrentCommand(string command, params string[] parameters);
```

Example usage (getting global throttle rates):
```cs
var commandList = new List<TorrentCommand>()
{
    new TorrentCommand("throttle.global_down.max_rate"),
    new TorrentCommand("throttle.global_up.max_rate")
};

var response = await _torrentService.SendTorrentQueryAsync(commandList,
    responseXml => new GlobalThrottleRates
    {
        DownloadRate = long.Parse(responseXml
        .ElementAt(0)
        .Descendants("value")
        .ElementAt(0).Value) / 1024,
        UploadRate = long.Parse(responseXml
        .ElementAt(1)
        .Descendants("value")
        .ElementAt(0).Value) / 1024
    });
```

- d.multicall
```cs
TorrentDMultiCallCommand(string view = "main", params string[] parameters):
```
Please see above example usage.

- f.multicall
```cs
TorrentFMultiCallCommand(string hash, params string[] parameters):
```

Example usage (getting files details within a torrent):

```cs
var commandList = new List<TorrentCommand>()
{
    new TorrentFMultiCallCommand(
            hash,
            "f.frozen_path=",
            "f.path=",
            "f.size_bytes=",
            "f.priority=",
            "f.size_chunks=",
            "f.completed_chunks="
        )
};

var response = await _torrentService.SendTorrentQueryAsync(commandList, responseXml
    => responseXml
        .ElementAt(0)
        .Descendants("data")
        .ElementAt(0)
        .Descendants("data")
        .Select((item, index) =>
        {
            var details = item.Descendants("value");

            return new TorrentFilesDetails
            {
                FileIndex = index,
                FrozenPath = details.ElementAt(0).Value.Trim(),
                Path = details.ElementAt(1).Value.Trim(),
                Size = long.Parse(details.ElementAt(2).Value.Trim()) / 1024,
                Priority = (FilePriority)int.Parse(details.ElementAt(3).Value.Trim()),
                TotalChunks = long.Parse(details.ElementAt(4).Value.Trim()),
                CompletedChunks = long.Parse(details.ElementAt(5).Value.Trim())
            };
        }));
```

#### Response

The response object has the following properties:

```cs
  public class rTorrentResponse<T>
    {
        public T? Body { get; private set; }
        public int Status { get; private set; }
        public string? ContentType { get; private set; }
        public long? ContentLength { get; private set; }
        public string? ErrorMessage { get; private set; }
    }
```
If the call to the rTorrent XMLRPC is successful the ErrorMessage property will be null while the Body property will be either the return type of the parser function or IEnumerable<XElement> (if no parser function is provided).

Status code will be 200 if call is successful.

In case of failure the status code and the error message will be what is returned by the rTorrent XMLRPC interface, while the Body property is null.

#### TODO

-   Implement more multicall helper objects (although other multicalls maybe made via general TorrentCommand object with setting the appropriate parameters in the right order)

## Disclaimer

Please note that I am not an actual programmer, I do coding solely as a hobby in my spare time so there is no guarantee that updates will be made or bugs will be fixed. I made this because I was not able to find any similar projects written in C#. Nevertheless, I am always open for constructive criticism. Also please be aware that this is my first public Github project :)
