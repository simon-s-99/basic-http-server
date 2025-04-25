using System.Net.Sockets;
using System.Net;

namespace HttpServer
{
    internal class Server
    {
        internal static void HttpServer()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 4221);
            server.Start();

            while (true)
            {
                var socket = server.AcceptSocket(); // wait for client
                Task.Run(() => Helpers.HandleSocket(socket));
            }
        }
    }
}
