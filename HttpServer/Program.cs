// A basic implementation of a http-server

using System.Net;
using System.Net.Sockets;

namespace HttpServer;

internal class Program
{
	static void Main(string[] args)
	{
		IPAddress listenerAddress = IPAddress.Any;
		int listenerPort = 3999;

		TcpListener listener = new(listenerAddress, listenerPort);
		listener.Start(); // start listening to incoming requests
		listener.AcceptSocket(); // accept connection requests
	}
}
