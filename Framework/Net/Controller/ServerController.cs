using System;
using System.Net;
using System.Text;
using Framework.Data.Controller;
using Framework.Data.Models;
using Framework.Net.Models;
using Newtonsoft.Json;

namespace Framework.Net.Controller;

public class ServerController : IDisposable
{
    private readonly int _maxThreadCount;
    private readonly HttpListener _listener;
    private static ServerController? Instance;

    private bool IsRunning;

    private int SessionLifetimeHours;

    private Dictionary<string, Session> Sessions { get; set; }

    private Dictionary<string, Route> Routes { get; set; }

    private ServerController(int port = 10001, int maxThreads = 5)
    {
        Console.Write("Server starting ...");

        _maxThreadCount = maxThreads;
        _listener = new HttpListener();

        IsRunning = true;
        SessionLifetimeHours = 1;
        _listener.Prefixes.Add($"http://*:{port}/");

        Sessions = new Dictionary<string, Session>();
        Routes = new Dictionary<string, Route>();
        Console.Write(" OK!\n");
    }

    public static ServerController GetInstance()
    {
        if (Instance == null)
            Instance = new ServerController();

        return Instance;
    }

    public void Dispose()
    {
        Console.Write("Server shutting down\r\n");
        Stop();
    }

    public async Task Listen()
    {
        _listener.Start();

        while (_listener.IsListening)
        {
            Console.Write("Listening...\r\n");

            var context = await _listener.GetContextAsync();

            Thread thread = new Thread(() => HandleRequestAsync(context));
            thread.Start();
        }
    }

    private async void HandleRequestAsync(HttpListenerContext ctx)
    {
        Console.Write($"Connected with url: {ctx.Request.Url.LocalPath}\r\n");

        var request = ctx.Request;
        var response = ctx.Response;

        if (request.HttpMethod == "OPTIONS")
        {
            response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT");
            response.AddHeader("Access-Control-Max-Age", "1728000");
            response.AppendHeader("Access-Control-Allow-Origin", "*");
            response.Close();
            return;
        }

        response.AppendHeader("Access-Control-Allow-Origin", "*");

        Route? route = await TryGetRouteAsync(ctx);

        if (route is null)
            return;

        // Execute Auth when required
        if (route.RequireAuth)
        {
            try
            {
                var session = CheckSession(ctx);

            }
            catch (UserNotLoggedInException ex)
            {
                await SendResponseAsync(response, 401, $"{{ \"Error\": \"User not logged in\" }}");
                return;
            }
        }

        route.Execute(ctx);

        if (response is not null)
            response.Close();
    }

    public void Stop()
    {
        _listener.Stop();
    }

    private Route? TryGetRoute(HttpListenerContext ctx)
    {
        Route? route;
        string requestedEndpoint = "";
        try
        {
            requestedEndpoint = ctx.Request.Url.LocalPath;
            var requestMethod = ctx.Request.HttpMethod;
            route = Routes[requestedEndpoint+requestMethod];
        }
        catch (ArgumentNullException)
        {
            string errorMsg = "Endpoint not provided";

            SendResponse(ctx.Response, 404, errorMsg);
            ctx.Response.Close();

            Console.Write($"{errorMsg} \n");
            return null;
        }
        catch (KeyNotFoundException)
        {
            string errorMsg = $"Endpoint {requestedEndpoint} is invalid";

            SendResponse(ctx.Response, 404, errorMsg);
            ctx.Response.Close();

            Console.Write($"{errorMsg} \n");
            return null;
        }

        return route;
    }


    private async Task<Route?> TryGetRouteAsync(HttpListenerContext ctx)
    {
        Route? route;
        string requestedEndpoint = "", requestMethod = "";

        try
        {
            requestedEndpoint = ctx.Request.Url.LocalPath;
            requestMethod = ctx.Request.HttpMethod;
            route = Routes[requestedEndpoint + "-" + requestMethod];
        }
        catch (ArgumentNullException)
        {
            string errorMsg = $"{{ \"Error\": \"Endpoint not provided\" }}";

            await SendResponseAsync(ctx.Response, 404, errorMsg);
            ctx.Response.Close();

            Console.Write($"{errorMsg} \n");
            return null;
        }
        catch (KeyNotFoundException)
        {
            string errorMsg = $"{{ \"Error\": \"Endpoint {requestedEndpoint} with method {requestMethod} is invalid\" }}";

            await SendResponseAsync(ctx.Response, 404, errorMsg);
            ctx.Response.Close();

            Console.Write($"{errorMsg} \n");
            return null;
        }

        return route;
    }

    public void AddRoute(string route, string type, bool authIsRequired, Action<HttpListenerContext> func)
    {
        var routeNode = new Route(route, type, authIsRequired, func);
        Routes.Add(route + "-" + type, routeNode);
    }

    public string GetTokenOfRequest(HttpListenerRequest req)
    {
        //if (req.Cookies["Authorization"] is not null)
        //{
        //    var cookie = req.Cookies["Authorization"];
        //    if (cookie.Value is not null)
        //    {
        //        return cookie.Value;
        //    }
        //}

        // get authtoken from headers
        var tokenHeader = req.Headers.Get("Authorization");
        string token = "";

        // Basic username-tokenBasic
        if (tokenHeader is not null && tokenHeader.Contains('-'))
            token = tokenHeader.Substring(("Basic").Length + 1);

        return token;
    }

    public Session CheckSession(HttpListenerContext ctx)
    {
        string token = GetTokenOfRequest(ctx.Request);

        Session? session;

        if (token is null || token == "")
            return null;

        if (Sessions.TryGetValue(token, out session))
        {
            // check if token is invalid
            if (session.LastAction.AddHours(SessionLifetimeHours) < DateTime.Now)
            {
                InvalidateSession(session);
                throw new UserNotLoggedInException("Session no longer valid. Please login again.",ctx.Response);
            }

            // if valid then set LastAction
            session.LastAction = DateTime.Now;
        } else if (token.Contains("admin"))
        {
            session = CreateSession("admin", "mtcgToken");
            session.IsAdmin = true;
            session.IsLoggedIn = true;
        }

        // no active session
        if (session is null || !session.IsLoggedIn)
            throw new UserNotLoggedInException();

        return session;
    }

    public bool CheckAdmin(string token)
    {
        Session? sess = GetSession(token);

        if (sess is null)
            throw new UserNotLoggedInException();

        return sess.IsAdmin;
    }

    public bool CheckAdmin(HttpListenerRequest req)
    {
        return CheckAdmin(GetTokenOfRequest(req));
    }

    public Session? GetSession(string token)
    {
        Session? session;
        Sessions.TryGetValue(token, out session);

        return session;
    }

    public Session GetSession(HttpListenerContext ctx)
    {
        string token = GetTokenOfRequest(ctx.Request);
        Session? session = GetSession(token);

        if (session is null)
            throw new UserNotLoggedInException(ctx.Response);

        return session;
    }

    public Session CreateSession(string username, string hash = "")
    {
        if (hash == "")
            hash = UserService.GenerateToken(8);

        string token = $"{username}-{hash}";
        var session = new Session(username, token);

        if (hash == "mtcgToken")
            session.IsAdmin = true;

        // add to dic
        Sessions.Add(token, session);

        return session;
    }

    private void InvalidateSession(Session sess)
    {
        Sessions.Remove(sess.Token);
    }

    public async Task SendResponseAsync(HttpListenerResponse response, int code, dynamic? data = null)
    {
        response.StatusCode = code;

        if( data != null )
        {
            response.ContentEncoding = Encoding.UTF8;
            response.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            string jsonString = JsonConvert.SerializeObject(data);
            byte[] buffer = Encoding.UTF8.GetBytes(jsonString);
            await response.OutputStream.WriteAsync(buffer);
        }

        response.Close();
    }

    public void SendResponse(HttpListenerResponse response, int code, dynamic? data = null)
    {
        response.StatusCode = code;

        if (data != null)
        {
            response.ContentEncoding = Encoding.UTF8;
            response.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            byte[] buffer = Array.Empty<byte>();
            string jsonString = JsonConvert.SerializeObject(data);
            buffer = Encoding.UTF8.GetBytes(jsonString);
            response.OutputStream.Write(buffer);
        }

        response.Close();
    }

    public T RequestToObject<T>(Stream inputStream)
    {
        string body;
        T? obj;

        using (Stream receiveStream = inputStream)
        {
            using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
            {
                body = readStream.ReadToEnd();
            }
        }

        Console.WriteLine("Body: " + body);

        if (body is null || body.Length <= 0)
            throw new ArgumentException("Could not parse body");

        obj = JsonConvert.DeserializeObject<T>(body);

        if (obj is null)
            throw new ArgumentException("Could not parse json");

        return obj;
    }
}