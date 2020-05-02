using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using static System.Text.RegularExpressions.Regex;

namespace rTorrentLib
{
    public class rTorrentResponse<T>
    {
        [MaybeNull]
        public T Body { get; } = default !;
        public int Status { get; }
        public string? ContentType { get; }
        public long? ContentLength { get; }
        public string? ErrorMessage { get; }

        public rTorrentResponse(int status, string errorMessage)
        {
            Status = status;
            ErrorMessage = errorMessage;
        }

        public rTorrentResponse(T body, int status)
        {
            Body = body;
            Status = status;
        }

        public rTorrentResponse(string response, Func<IEnumerable<XElement>, T> parser)
        {
            var responseLines = Split(response, $"[{Environment.NewLine}]+");
            Status = int.Parse(responseLines[0].Split(@" ".ToCharArray())[1]);
            ContentType = Split(responseLines[1], @": ")[1].Trim();
            ContentLength = long.Parse(Split(responseLines[2], @": ")[1]);

            var xmlDoc = XElement.Parse(Replace(response, "Status.*|Content-Type.*|Content-Length.*|", "").Trim());
            if (xmlDoc.XPathSelectElements("//struct").Count() > 0)
            {
                var members = xmlDoc.XPathSelectElements("//member/value").ToList();
                Status = Math.Abs(Convert.ToInt16(members[0].Value));
                ErrorMessage = members[1].Value;
            }
            else
            {
                var responseString = xmlDoc.XPathSelectElements("//param/value/array/data/value/array/data");
                Body = parser(responseString);
            }
        }
    }
}