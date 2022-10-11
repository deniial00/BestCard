using System.Text;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace Client.Controller;

class ClientController
{
    private TcpClient _tcpClient;
    private readonly string _token;

    public ClientController(string iPAddress = "localhost", int port = 1001)
    {
        _token = "";

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
        var res = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
        return res != null ? res : throw new ArgumentException("Could not deserelize JSON");

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