using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace rTorrentLib
{
    public class rTorrentClient
    {
        private readonly string IpAddress;
        private readonly int Port;

        public rTorrentClient(string ipAddress = "127.0.0.1", int port = 5000)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        public async Task<string> Connect(string xmlCommand)
        {
            string response;
            try
            {
                using(var tcpClient = new TcpClient(IpAddress, Port))
                {
                    var networkStream = tcpClient.GetStream();
                    var streamWriter = new StreamWriter(networkStream);
                    streamWriter.AutoFlush = true;
                    var streamReader = new StreamReader(networkStream);

                    var header = $"CONTENT_LENGTH{'\0'}{Encoding.ASCII.GetBytes(xmlCommand).Length}{'\0'}SCGI{'\0'}1{'\0'}UNTRUSTED_CONNECTION{'\0'}1";

                    await streamWriter.WriteAsync($"{header.Length}:{header},{xmlCommand}");

                    response = await streamReader.ReadToEndAsync();

                    streamWriter.Close();
                    tcpClient.Close();
                    return response;
                };
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
                                                <string>Connection refused at {IpAddress}:{Port} - Maybe rTorrent is down?</string>
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