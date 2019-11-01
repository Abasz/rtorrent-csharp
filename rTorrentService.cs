using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

using rTorrentLib.Interfaces;
using rTorrentLib.Model;

namespace rTorrentLib.Services
{
    public class rTorrentService : IrTorrentService
    {
        private readonly string _ipAddress;
        private readonly int _port;

        public rTorrentService(string ipAddress = "127.0.0.1", int port = 5000)
        {
            _ipAddress = ipAddress;
            _port = port;
        }
        public Task<rTorrentResponse<T>> SendTorrentQueryAsync<T>(List<TorrentCommand> commands, Func<IEnumerable<XElement>, T> parser) => SendQuery(commands, parser);

        public Task<rTorrentResponse<IEnumerable<XElement>>> SendTorrentQueryAsync(List<TorrentCommand> commands) => SendQuery(commands, (e) => e);

        private async Task<rTorrentResponse<T>> SendQuery<T>(List<TorrentCommand> commands, Func<IEnumerable<XElement>, T> parser)
        {
            var torrenCallCommand = new rTorrentRequest(commands, _ipAddress, _port);

            return await torrenCallCommand.SendCommand(parser);
        }
    }
}