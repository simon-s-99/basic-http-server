using System.Net.Sockets;
using System.Net;

namespace HttpServer
{
    public class Server
    {
        /// <summary>
        /// Start HttpServer on localhost with default port 3999 or specified ip-address & port.
        /// </summary>
        /// <param name="ipAddress">IP Adress that server should listen on. (default: localhost/*)</param>
        /// <param name="port">Port that the server should listen on. (default: 3999)</param>
        public static void HttpServer(IPAddress? ipAddress = null, int? port = null)
        {
            TcpListener server = new TcpListener(ipAddress ?? IPAddress.Any, port ?? 3999);
            server.Start();

            while (true)
            {
                var socket = server.AcceptSocket(); // wait for client
                Task.Run(() => Helpers.HandleSocket(socket));
            }
        }
    }
}
