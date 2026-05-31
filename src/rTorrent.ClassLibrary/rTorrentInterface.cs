using System.Xml.Linq;

namespace rTorrentLib.Interfaces
{
    public interface IrTorrentService
    {
        Task<rTorrentResponse<T>> SendTorrentQueryAsync<T>(List<TorrentCommand> commands, Func<IEnumerable<XElement>, T> parser);
        Task<rTorrentResponse<IEnumerable<XElement>>> SendTorrentQueryAsync(List<TorrentCommand> commands);
    }
}