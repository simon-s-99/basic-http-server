using System.Net;

namespace HttpServerTests.GETTests
{
    public class GET
    {
        [Fact]
        public void BasicGET_ExpectOK()
        {
            // ARRANGE
            IPAddress ipAddress = IPAddress.Any;
            int port = 4001;

            Uri uri = new($"http://{ipAddress.ToString()}:{port.ToString()}/");
            HttpRequestMessage request = new(HttpMethod.Get, uri);

            // ACT
            HttpServer.Server.HttpServer(ipAddress, port);
            HttpResponseMessage resp;
            using (HttpClient client = new())
            {
                resp = client.Send(request);

            }

            // ASSERT
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }
    }
}
