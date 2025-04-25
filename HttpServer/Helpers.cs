using System.Net.Sockets;
using System.Text;

namespace HttpServer
{
    internal class Helpers
    {
        internal static Task HandleSocket(Socket socket)
        {
            string ok = "HTTP/1.1 200 OK\r\n";
            string created = "HTTP/1.1 201 Created\r\n";
            string badRequest = "HTTP/1.1 400 Bad Request\r\n";
            string notFound = "HTTP/1.1 404 Not Found\r\n";
            string conflict = "HTTP/1.1 409 Conflict\r\n";

            string contentTypeTextPlain = "Content-Type: text/plain\r\n";
            string contentTypeOctetStream = "Content-Type: application/octet-stream\r\n";

            byte[] receiveBuffer = new byte[1024]; // 1024 bytes = 1 kilobyte
            socket.Receive(receiveBuffer);

            string[] received = Encoding.UTF8.GetString(receiveBuffer).Split("\r\n");
            string[] requestLines = received[0].Split(' ');

            // get all received headers as a Dict. but skip first & last element (request line & body)
            Dictionary<string, string> requestHeaders = received.Skip(1).SkipLast(1).ToDictionary(
                keySelector: k =>
                {
                    string[] kSplit = k.Split(": ");
                    return kSplit.Any() ? kSplit[0] : string.Empty;
                },
                elementSelector: v =>
                {
                    string[] vSplit = v.Split(": ");
                    return vSplit.Length == 2 ? vSplit[1] : "";
                });

            string requestBody = received.LastOrDefault("");

            string requestMethod = requestLines[0];
            string requestTarget = requestLines[1];
            // ...

            string statusLine = string.Empty;
            string responseHeaders = string.Empty;
            string responseBody = string.Empty;

            if (requestMethod.StartsWith("GET"))
            {
                if (requestTarget.Contains("/echo/"))
                {
                    statusLine = ok;
                    responseHeaders += contentTypeTextPlain;
                    responseBody = requestTarget.Remove(requestTarget.IndexOf("/echo/"), "/echo/".Length);
                    responseHeaders += $"Content-Length: {Encoding.Default.GetByteCount(responseBody)}\r\n";
                }
                else if (requestTarget.Contains("/user-agent"))
                {
                    requestHeaders.TryGetValue("User-Agent", out string? userAgentHeader);

                    if (!string.IsNullOrEmpty(userAgentHeader))
                    {
                        statusLine = ok;
                        responseHeaders += contentTypeTextPlain;
                        responseBody = userAgentHeader!;
                        responseHeaders += $"Content-Length: {Encoding.Default.GetByteCount(responseBody)}\r\n";
                    }
                    else
                    {
                        statusLine = notFound;
                    }
                }
                else if (requestTarget.Contains("/files/"))
                {
                    string filePath = GetRequestedFilePath(requestTarget.Split('/').LastOrDefault("*"));

                    if (File.Exists(filePath))
                    {
                        string fileTextContent = File.ReadAllText(filePath);

                        statusLine = ok;
                        responseHeaders += contentTypeOctetStream;
                        responseBody = fileTextContent;
                        responseHeaders += $"Content-Length: {Encoding.Default.GetByteCount(responseBody)}\r\n";
                    }
                    else { statusLine = notFound; }
                }
                else if (requestTarget == "/" || requestTarget == string.Empty) { statusLine = ok; }
                else { statusLine = notFound; }
            }
            else if (requestMethod.StartsWith("POST"))
            {
                if (requestTarget.StartsWith("/files/"))
                {
                    requestHeaders.TryGetValue("Content-Type", out string? requestContentType);
                    requestHeaders.TryGetValue("Content-Length", out string? requestContentLength);

                    if (string.IsNullOrEmpty(requestContentType)
                        || string.IsNullOrEmpty(requestContentLength)
                        || string.IsNullOrEmpty(requestBody))
                    {
                        // guard clause
                        statusLine = badRequest;
                    }
                    else
                    {
                        string filePath = GetRequestedFilePath(requestTarget.Split('/').LastOrDefault("*"));

                        if (File.Exists(filePath))
                        {
                            // guard clause
                            statusLine = conflict;
                        }
                        else
                        {
                            // file does not already exist so go ahead & write
                            using (StreamWriter outputFile = new(filePath))
                            {
                                if (int.TryParse(requestContentLength, out int requestContentLengthAsInt))
                                {
                                    StringBuilder contentToWrite = new(
                                        requestBody,
                                        0,
                                        requestContentLengthAsInt,
                                        requestContentLengthAsInt);
                                    outputFile.Write(contentToWrite);
                                }
                                // else not required, ternary operator will not find file
                            }

                            // check if file was written successfully
                            statusLine = File.Exists(filePath) ? created : conflict;
                        }
                    }
                }
            }
            else
            {
                statusLine = notFound;
            }

            // header section ends with CRLF, the amount of responseHeaders do not matter 
            string responseToSend = statusLine + responseHeaders + "\r\n" + responseBody;
            socket.Send(Encoding.UTF8.GetBytes(responseToSend));

            return Task.CompletedTask;
        }

        private static string GetRequestedFilePath(string requestedFileName)
        {
            string envPath = Environment.GetCommandLineArgs()[0];
            string dllEnvPath = envPath.Remove(envPath.LastIndexOfAny(['/', '\\']));

            // file cannot be named '*' so default value will always cause failure
            //string filePath = dllEnvPath + '\\' + requestedFileName;
            string filePath = Path.Combine(dllEnvPath, requestedFileName);
            return filePath;
        }
    }
}
