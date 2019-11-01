using System.Collections.Generic;

namespace rTorrentLib.Model
{
    public class TorrentCommand
    {
        public string Command { get; }
        public string[] Parameters { get; }

        public TorrentCommand(string command, params string[] parameters)
        {
            Command = command;
            Parameters = parameters;
        }
    }

    public class TorrentDMultiCallCommand : TorrentCommand
    {
        public string View { get; }

        public TorrentDMultiCallCommand(string view = "main", params string[] parameters):
            base("d.multicall2", AddParameters(view, parameters))
            {
                View = view;
            }

        private static string[] AddParameters(string view, string[] parameters)
        {
            var parametersBase = new List<string> { "", view };
            parametersBase.AddRange(parameters);

            return parametersBase.ToArray();
        }

    }

    public class TorrentFMultiCallCommand : TorrentCommand
    {
        public string Hash { get; }
        public TorrentFMultiCallCommand(string hash, params string[] parameters):
            base("f.multicall", AddParameters(hash, parameters))
            {
                Hash = hash;
            }

        private static string[] AddParameters(string hash, string[] parameters)
        {
            var parametersBase = new List<string> { hash, "" };
            parametersBase.AddRange(parameters);

            return parametersBase.ToArray();
        }
    }
}