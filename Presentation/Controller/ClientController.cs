using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Presentation.Controller;

class ClientController
{
    private TcpClient _tcpClient;
    private readonly string _token;

    public ClientController(string iPAddress = "localhost", int port = 1001)
    {
        Console.Write("Client starting ...");
        try
        {
            _tcpClient = new TcpClient(iPAddress, port);
        }
        catch (Exception ex)
        {
            //log
            Console.WriteLine($"Error at creating HttpClient\nError: {ex.Message}");
            throw;
        }
        Console.WriteLine(" OK!\n");
    }

    ~ClientController()
    {
        _tcpClient.Dispose();
        _tcpClient = null;
    }

    private Dictionary<string, dynamic> Request(Dictionary<string, dynamic> message)
    {
        var nwStream = _tcpClient.GetStream();

        // write
        
        byte[] bytesToSend = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        nwStream.Write(bytesToSend, 0, bytesToSend.Length);

        // read
        byte[] bytesToRead = new byte[_tcpClient.ReceiveBufferSize];
        int bytesRead = nwStream.Read(bytesToRead, 0, _tcpClient.ReceiveBufferSize);
        string response = Encoding.UTF8.GetString(bytesToRead, 0, bytesRead);

        // return res
        return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);

    }

    public Dictionary<string, dynamic> AuthUser(string username, string password)
    {
        var request = new Dictionary<string, dynamic>();
        request.Add("mode", "auth");
        request.Add("username", username);
        request.Add("password", password);
        
        var response = Request(request);

        return response;
    }
}