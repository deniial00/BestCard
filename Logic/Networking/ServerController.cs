using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Net;


using Logic.Networking.Models;

namespace Logic.Networking;

class ServerController
{
    private TcpListener _tcpListener;
    private bool _isRunning;
    private Dictionary<string, Session> _sessions;
    public ServerController(int port = 1001)
    {
        Console.Write("Server starting ...");

        _tcpListener = new TcpListener(IPAddress.Any, port);
        _sessions = new Dictionary<string, Session>();
        _isRunning = true;

        Console.Write(" OK!\n");
    }

    public async Task Listen()
    {
        _tcpListener.Start();

        while (_isRunning)
        {
            Console.WriteLine("listening...");
            TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync();
            Console.WriteLine("connected");
            Task.Factory.StartNew(() => HandleConnection(tcpClient));


        }
    }

    public void Stop()
    {
        _tcpListener.Stop();
    }

    public void HandleConnection(TcpClient client)
    {
        var stream = client.GetStream();

        if (stream.DataAvailable)
        {
            // get stream
            var nwStream = client.GetStream();

            // create buffer for receiving and sending bytes
            byte[] buffer = new byte[client.ReceiveBufferSize];

            // read from stream and get data
            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
            string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            var req1 = new HttpRequest(data, HTTPMessageType.Request);
            
            Console.Write(data);
            Thread.Sleep(2000);
            // convert data into beautiful dict
            var req = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(data);

            // Check for valid session and add token to response 
            var res = new Dictionary<string, dynamic>();
            //Session? session = CheckSession(req, client);
            //res.Add("token", session.Token);

            //switch (req["mode"])
            //{
            //    case "auth":
            //        if (req.ContainsKey("username") && req.ContainsKey("password"))
            //        {
            //            Console.WriteLine($"[{Task.CurrentId}] User authentication: {req["username"]} : {req["password"]}");
                            
            //            LogOn(session, req["username"], req["password"]);
            //            res.Add("logged_in", "true");

            //        }
            //        else
            //        {
            //            res.Add("error", "Missing username or password");
            //        }
            //        break;
            //    case "trade":
            //        break;
            //    case "battle":
            //        break;
            //    case "profile":
            //        break;
            //}


            // clear buffer
            Array.Clear(buffer, 0, client.ReceiveBufferSize);

            // convert to sendable data
            var json = JsonConvert.SerializeObject(res);
            buffer = Encoding.UTF8.GetBytes(json);

            // write to stream
            nwStream.Write(buffer, 0, buffer.Length);
            nwStream.Close();

        }
    }

    private Session CheckSession(Dictionary<string, dynamic> json, TcpClient client)
    {
        if (json.ContainsKey("token") && _sessions.ContainsKey(json["token"]))
            return _sessions[json["token"]];

        string token = GenerateToken64();
        var session = new Session(token);

        _sessions.Add(token, session);
        return session;
    }

    private int LogOn(Session session, string username, string password)
    {
        if (username != "deniial" && password != "123456")
        {
            return -1;
        }

        session.IsLoggedIn = true;
        return 1;
    }

    private string GenerateToken64()
    {
        const string src = "abcdefghijklmnopqrstuvwxyz0123456789";
        int length = 64;
        var sb = new StringBuilder();
        Random RNG = new Random();
        for (var i = 0; i < length; i++)
        {
            var c = src[RNG.Next(0, src.Length)];
            sb.Append(c);
        }
        return sb.ToString();
    }
}