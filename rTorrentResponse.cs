using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using static System.Text.RegularExpressions.Regex;

namespace rTorrentLib
{
    public class rTorrentResponse<T>
    {
        public T Body { get; private set; }
        public int Status { get; private set; }
        public string ContentType { get; private set; }
        public long ContentLength { get; private set; }
        public string ErrorMessage { get; private set; }

        public rTorrentResponse(T body, int status, string errorMessage)
        {
            Body = body;
            Status = status;
            ErrorMessage = errorMessage;
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
                ParseErrorResponse(xmlDoc);
            }
            else
            {
                var responseString = xmlDoc.XPathSelectElements("//param/value/array/data/value/array/data");

                Body = parser(responseString);
            }
        }

        private void ParseErrorResponse(XElement xmlDoc)
        {
            var members = xmlDoc.XPathSelectElements("//member/value").ToList();
            Status = Math.Abs(Convert.ToInt16(members[0].Value));
            ErrorMessage = members[1].Value;
        }
    }
}