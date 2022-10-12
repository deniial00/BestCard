﻿using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using Framework.Networking.Models;
using Framework.Networking.HTTPComponents;
using Framework.Networking.HTTPComponents.Enums;
using HttpStatusCode = Framework.Networking.HTTPComponents.Enums.HttpStatusCode;

namespace Framework.Networking.Controller;

class ServerController
{
    private TcpListener _tcpListener;
    private bool _isRunning;

    private Dictionary<string, Session> Sessions { get; set; }

    private Dictionary<string, Route> Routes { get; set; }

    public ServerController(int port = 1001)
    {
        Console.Write("Server starting ...");

        _isRunning = true;
        _tcpListener = new TcpListener(IPAddress.Loopback, port);
        Sessions = new Dictionary<string, Session>();

        Console.Write(" OK!\n");
    }
    ~ServerController()
    {
        Console.Write("Server shutting down\r\n");
    }

    public void Start()
    {
        _tcpListener.Start();

        while (_isRunning)
        {
            Console.Write("listening...");
            
            TcpClient tcpClient = _tcpListener.AcceptTcpClient();
            var tcpStream = tcpClient.GetStream();
            Console.Write("connected\r\n\r\n");
            
            if(HandleConnection(tcpStream) < 1)
            {
                Console.WriteLine("ERROR!");
            }

            tcpClient.Close();
        }
    }

    public void Stop()
    {
        _tcpListener.Stop();
    }

    public int HandleConnection(NetworkStream nwStream)
    {
        // create buffer for receiving and sending bytes
        byte[] buffer = new byte[1024];

        // read from stream and get data
        int bytesRead = nwStream.Read(buffer, 0, 1024);
        try
        {
            var req = new HttpRequest(buffer, bytesRead);
            Console.Write(req.ToString());
        }
        catch (ArgumentException)
        {
            // try sending Bad Request
            bool success = TryAbortConnection(nwStream, HttpStatusCode.BadRequest);

            if (!success)
                throw;
        }
        catch (NotSupportedException)
        {
            // try sending Bad Request
            bool success = TryAbortConnection(nwStream, HttpStatusCode.HTTPVersionNotSupported);

            if (!success)
                throw;
        }
        catch (Exception)
        {
            // try sending Bad Request
            bool success = TryAbortConnection(nwStream, HttpStatusCode.InternalServerError);

            if (!success)
            {
                return -1;
            }
        }

        // Check for valid session and add token to response 
        var res = new HttpResponse();
        res.SetStatusCode(HttpStatusCode.OK);

        var resBody = new Dictionary<string, dynamic>();

        //Session? session = CheckSession(req);
        //res.Add("token", session.Token;

        // convert to sendable data
        // var json = JsonConvert.SerializeObject(res);

        // clear buffer
        Array.Clear(buffer, 0, 1024);
        buffer = res.ToBytes();

        Console.Write(res.ToString());
        // write to stream
        nwStream.Write(buffer, 0, buffer.Length);
        nwStream.Close();

        return 1;

    }

    private Session CheckSession(Dictionary<string, dynamic> json)
    {
        if (json.ContainsKey("token") && Sessions.ContainsKey(json["token"]))
            return Sessions[json["token"]];

        string token = GenerateToken64();
        var session = new Session(token);

        Sessions.Add(token, session);
        return session;
    }

    public bool TryAbortConnection(NetworkStream stream, HttpStatusCode statusCode)
    {
        // create response
        var res = new HttpResponse();
        res.SetStatusCode(statusCode);

        // convert res to bytes
        byte[] buffer = res.ToBytes();
        stream.Write(buffer, 0, buffer.Length);
        
        // close stream
        stream.Close();

        return true;
    }

    public void AddRoute(string route, HttpMethodType type, Func<HttpRequest, HttpResponse> func)
    {
        var routeNode = new Route(route, type, func);
        Routes.Add(route, routeNode);
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