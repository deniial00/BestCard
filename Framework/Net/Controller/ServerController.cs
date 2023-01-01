using System.Net;
using System.Text;
using Framework.Data.Controller;
using Framework.Data.Models;
using Framework.Networking.Models;
using Newtonsoft.Json;

namespace Framework.Networking.Controller;

class ServerController : IDisposable
{
    private readonly int _maxThreadCount;
    private readonly HttpListener _listener;


    private bool IsRunning;

    private int SessionLifetimeHours;

    private Dictionary<string, Session> Sessions { get; set; }

    private Dictionary<string, Route> Routes { get; set; }

    public ServerController(int port = 10001, int maxThreads = 5)
    {
        // https://stackoverflow.com/questions/4672010/multi-threading-with-net-httplistener
        //_stopEvent = new ManualResetEvent(false);
        //_idleEvent = new ManualResetEvent(false);
        //_busy = new Semaphore(_maxThreadCount, _maxThreadCount);
        //_listenerThread = new Thread(HandleRequestsAsync);

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
    public void Dispose()
    {
        Console.Write("Server shutting down\r\n");
        Stop();
    }

    public void HandleRequests()
    {
        _listener.Start();

        while (_listener.IsListening)
        {
            Console.Write("Listening...\r\n");

            var context = _listener.GetContext();
            var request = context.Request;
            var response = context.Response;

            Console.Write($"Connected with url: {context.Request.Url}\r\n");

            Route? route = TryGetRoute(context);

            if (route is null)
                continue;

            route.Execute(request, response);

            if(response is not null)
                response.Close();
        }
    }

    public async Task HandleRequestsAsync()
    {
        _listener.Start();

        while (_listener.IsListening)
        {
            Console.Write("Listening...\r\n");

            var context = await _listener.GetContextAsync();

            await Task.Run(async () =>
            {
                Console.Write($"Connected with url: {context.Request.Url}\r\n");

                var request = context.Request;
                var response = context.Response;
                Route? route = await TryGetRouteAsync(context);

                if (route is null)
                    return;

                route.Execute(request, response);

                if (response is not null)
                    response.Close();

            }).ConfigureAwait(false);

        }
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
            route = Routes[requestedEndpoint];
        }
        catch (ArgumentNullException ex)
        {
            string errorMsg = "Endpoint not provided";

            SendResponse(ctx.Response, 404, errorMsg);
            ctx.Response.Close();

            Console.Write($"{errorMsg} \n");
            return null;
        }
        catch (KeyNotFoundException ex)
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
        string requestedEndpoint = "";
        try
        {
            requestedEndpoint = ctx.Request.Url.LocalPath;
            route = Routes[requestedEndpoint];
        }
        catch (ArgumentNullException ex)
        {
            string errorMsg = "Endpoint not provided";

            await SendResponseAsync(ctx.Response, 404, errorMsg);
            ctx.Response.Close();

            Console.Write($"{errorMsg} \n");
            return null;
        }
        catch (KeyNotFoundException ex)
        {
            string errorMsg = $"Endpoint {requestedEndpoint} is invalid";

            await SendResponseAsync(ctx.Response, 404, errorMsg);
            ctx.Response.Close();

            Console.Write($"{errorMsg} \n");
            return null;
        }

        return route;
    }

    public void AddRoute(string route, HttpMethod type, bool authIsRequired, Action<HttpListenerRequest, HttpListenerResponse> func)
    {
        var routeNode = new Route(route, type, authIsRequired, func);
        Routes.Add(route, routeNode);
    }

    public Session CheckSession(UserCredentials cred, string token)
    {

        Session session;

        if(token == "Authorization: Basic admin-mtcgToken")

        if (token != "")
        {
            if (Sessions.TryGetValue(token, out session))
            {
                // check if token is invalid
                if (session.LastAction.AddHours(SessionLifetimeHours) > DateTime.Now)
                {
                    InvalidateSession(session);
                    throw new Exception("Session no longer valid");
                }
                // if valid then set LastAction
                session.LastAction = DateTime.Now;
                return session;
            } else
            {
                token = UserService.GenerateToken64();
            }
        }

        session = new Session(token);

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
            byte[] buffer = Array.Empty<byte>();
            string jsonString = JsonConvert.SerializeObject(data);
            buffer = Encoding.UTF8.GetBytes(jsonString);
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
            byte[] buffer = Array.Empty<byte>();
            string jsonString = JsonConvert.SerializeObject(data);
            buffer = Encoding.UTF8.GetBytes(jsonString);
            response.OutputStream.Write(buffer);
        }

        response.Close();
    }

    public T RequestToObject<T>(HttpListenerRequest req)
    {
        string body;
        T obj;

        using (Stream receiveStream = req.InputStream)
        {
            using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
            {
                body = readStream.ReadToEnd();
            }
        }

        if (body is null || body.Length <= 0)
            throw new ArgumentException("Could not parse body");

        obj = JsonConvert.DeserializeObject<T>(body);

        if (obj is null)
            throw new ArgumentException("Could not parse json");

        return obj;
    }
}