// A basic implementation of a http-server

using System.Text; // used for conversions to/from byte[]/string w/ encoding
using System.Net;
using System.Net.Sockets;

namespace HttpServer;

public class Program
{
	public static void Main(string[] args)
	{
		IPAddress listenerAddress = IPAddress.Any;
		int listenerPort = 3999;

		TcpListener listener = new(listenerAddress, listenerPort);
		listener.Start(); // start listening to incoming requests

		// accept connection requests & store connection-socket in a variable
		var socket = listener.AcceptSocket(); 

		// response message explanation:
		// HTTP/1.1 = http version
		// 200 = status code
		// OK = reason phrase (optional)
		// \r\n marks end of section (a http response consists of 3 parts: status, headers, response body)
		// NO HEADERS (but we still end this required section with \r\n)
		byte[] responseBuffer = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n\r\n");

		socket.Send(responseBuffer);

		//Console.ReadLine();
	}
}
