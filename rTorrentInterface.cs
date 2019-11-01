using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

using rTorrentLib.Model;

namespace rTorrentLib.Interfaces
{
    public interface IrTorrentService
    {
        Task<rTorrentResponse<T>> SendTorrentQueryAsync<T>(List<TorrentCommand> commands, Func<IEnumerable<XElement>, T> parser);
        Task<rTorrentResponse<IEnumerable<XElement>>> SendTorrentQueryAsync(List<TorrentCommand> commands);
    }
}