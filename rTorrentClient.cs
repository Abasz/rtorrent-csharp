using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace rTorrentLib
{
    public class rTorrentClient
    {
        private readonly string _ipAddress;
        private readonly int _port;

        public rTorrentClient(string ipAddress = "127.0.0.1", int port = 5000)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public async Task<string> Connect(string xmlCommand)
        {
            try
            {
                using var tcpClient = new TcpClient(_ipAddress, _port);
                var networkStream = tcpClient.GetStream();
                var streamReader = new StreamReader(networkStream);

                var header = $"CONTENT_LENGTH\0{Encoding.UTF8.GetBytes(xmlCommand).Length}\0CONTENT_TYPE\0text/xml\0SCGI\01\0UNTRUSTED_CONNECTION\01\0";

                var request = $"{Encoding.UTF8.GetBytes(header).Length}:{header},{xmlCommand}";
                var requestBytes = Encoding.UTF8.GetBytes(request);

                await networkStream.WriteAsync(requestBytes, 0, requestBytes.Length);

                return await streamReader.ReadToEndAsync();
            }
            catch
            {
                return $@"Status: 200 OK
                Content-Type: text/xml
                Content-Length: 369
                <?xml version='1.0' encoding='UTF-8'?>
                <methodResponse>
                    <params>
                        <param>
                            <value>
                                <array>
                                <data>
                                    <value>
                                        <struct>
                                            <member>
                                            <name>faultCode</name>
                                            <value>
                                                <i4>503</i4>
                                            </value>
                                            </member>
                                            <member>
                                            <name>faultString</name>
                                            <value>
                                                <string>Connection refused at {_ipAddress}:{_port} - Maybe rTorrent is down?</string>
                                            </value>
                                            </member>
                                        </struct>
                                    </value>
                                </data>
                                </array>
                            </value>
                        </param>
                    </params>
                </methodResponse>";
            }
        }
    }
}