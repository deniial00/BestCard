
using System.Net;
using System.Net.Sockets;

namespace Presentation.Controller;

internal class HttpClientController
{
    private Socket _tcpClient;
    HttpClientController(string ip = "127.0.0.1", int port = 8000)
    {
        try
        {
            IPEndPoint ipEndPoint = new(IPAddress.Parse(ip), port);
            var listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(ipEndPoint);
        }
        catch (SocketException ex)
        {
            //log
            Console.WriteLine($"Error at creating Socket\nError: {ex.Message}");
        }
    }

    ~HttpClientController()
    {
        _tcpClient.Dispose();
        _tcpClient = null;
    }

    private async Task Request()
    {

    }
}
