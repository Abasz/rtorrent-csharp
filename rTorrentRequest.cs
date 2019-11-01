using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using rTorrentLib.Model;

namespace rTorrentLib
{
    public class rTorrentRequest
    {
        public List<TorrentCommand> TorrentCommands { get; }
        public string CommandString { get; }
        public rTorrentClient TorrentClient { get; }

        public rTorrentRequest(List<TorrentCommand> torrentCommands, string ipAddress = "127.0.0.1", int port = 5000)
        {
            if (torrentCommands.Count == 0) throw new NullReferenceException("XMLRPC command is missing");
            TorrentClient = new rTorrentClient(ipAddress, port);
            TorrentCommands = torrentCommands;
            CommandString = XmlRpcTorrentSerialize();
        }

        public async Task<rTorrentResponse<T>> SendCommand<T>(Func<IEnumerable<XElement>, T> parser)
        {
            var response = TorrentClient.Connect(CommandString);

            return new rTorrentResponse<T>(await response, parser);
        }

        private string XmlRpcTorrentSerialize()
        {
            var str = new StringBuilder();

            str.Append($"<?xml version='1.0' encoding='iso-8859-1'?><methodCall><methodName>");

            str.Append($"system.multicall</methodName><params><param><value><array><data>");
            foreach (var command in TorrentCommands)
            {
                str.Append($"<value><struct><member><name>methodName</name><value><string>{command.Command}</string></value></member><member><name>params</name><value><array><data>");

                foreach (var param in command.Parameters)
                {
                    str.Append($"<value><{GetParameterType(param)}>{param}</{GetParameterType(param)}></value>");
                }
                str.Append("</data></array></value></member></struct></value>");
            }
            str.Append($"</data></array></value></param>");
            str.Append("</params></methodCall>");
            return str.ToString();
        }

        protected static string GetParameterType(object parameter)
        {
            if ((parameter is long))
                return "i4";
            if (parameter is float)
                return "i8";
            return "string";
        }
    }
}