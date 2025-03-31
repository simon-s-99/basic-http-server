using System.Net;
using System.Net.Sockets;

// A basic implementation of a http-server

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

        // receive request info from client
        var requestBuffer = new byte[1024];
        int requestBytes = socket.Receive(requestBuffer);
        var requestLines = System.Text.Encoding.UTF8.GetString(requestBuffer).Split("\r\n"); // \r\n marks end of section

        var firstLine = requestLines[0].Split(' ');
        var (method, path, httpVersion) = (firstLine[0], firstLine[1], firstLine[2]);

        // response message explanation:
        // HTTP/1.1 = http version
        // 200 = status code
        // OK = reason phrase (optional)
        // [status text here]

        string response = path == "/" ? $"{httpVersion} 200 OK\r\n\r\n"
            : $"{httpVersion} 404 Not Found\r\n\r\n";

        socket.Send(System.Text.Encoding.UTF8.GetBytes(response));
    }
}
