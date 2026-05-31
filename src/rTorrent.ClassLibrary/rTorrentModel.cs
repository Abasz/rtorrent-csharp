namespace rTorrentLib.Model
{
    public class TorrentCommand
    {
        public string Command { get; }
        public List<TorrentParams> Parameters { get; }

        public TorrentCommand(string command, params string[] parameters)
        {
            Command = command;
            Parameters = parameters.Select(param => new TorrentParams(param)).ToList();
        }

        public TorrentCommand(string command, List<TorrentParams> parameters)
        {
            Command = command;
            Parameters = parameters;
        }
    }

    public class TorrentParams
    {
        public object Parameter { get; } = null!;
        public string Type { get; }

        public TorrentParams(object parameter, string? type = null)
        {
            Type = type
            switch
            {
                null when parameter is int || parameter is long => "i4",
                null when parameter is double || parameter is float => "i8",
                null => "string",
                _ => type,
            };
            Parameter = parameter;
        }

    }

    public class TorrentDMultiCallCommand : TorrentCommand
    {
        public string View { get; }

        public TorrentDMultiCallCommand(string view = "main", params string[] parameters) :
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
        public TorrentFMultiCallCommand(string hash, params string[] parameters) :
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