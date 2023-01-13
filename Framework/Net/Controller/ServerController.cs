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

    //public ServerController()
    //{
    //}

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
                Console.Write($"Connected with url: {context.Request.Url.LocalPath}\r\n");

                var request = context.Request;
                var response = context.Response;
                Route? route = await TryGetRouteAsync(context);

                if (route is null)
                    return;

                // Execute Auth when required
                if (route.RequireAuth)
                {
                    var session = CheckSession(request);
                    if (session is null || !session.IsLoggedIn)
                        throw new UserNotLoggedInException();
                }

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

    public string GetTokenOfRequest(HttpListenerRequest req)
    {
        // get authtoken from headers
        var tokenHeader = req.Headers.Get("Authorization");
        string token = "";

        // Basic username-tokenBasic
        if (tokenHeader is not null && tokenHeader.Contains('-'))
            token = tokenHeader.Substring(("Basic").Length + 1);

        return token;
    }

    public Session? CheckSession(HttpListenerRequest req)
    {
        string token = GetTokenOfRequest(req);

        Session? session;


        if (token is null || token == "")
            return null;

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
        }
        

        if (token.Substring(token.IndexOf("-") + 1) == "mtcgToken")
        {
            session = Session.AdminUserSession();
            Console.WriteLine("Creating Admin Session");
            Sessions.Add(token, session);
        }

        return session;
    }

    public Session? GetSession(string token)
    {
        Session session;
        Sessions.TryGetValue(token, out session);

        return session;
    }

    public Session CreateSession(UserCredentialModel cred, HttpListenerResponse res)
    {
        string token = $"{cred.Username}-{UserService.GenerateToken64()}";
        var session = new Session(cred, token);

        // add to dic and add header
        Sessions.Add(token, session);
        res.Cookies.Add(new Cookie("Authorization", $"Basic {token}"));

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

    public T RequestToObject<T>(Stream inputStream)
    {
        string body;
        T obj;

        using (Stream receiveStream = inputStream)
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