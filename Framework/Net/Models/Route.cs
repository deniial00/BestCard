using System.Net;

namespace Framework.Net.Models;

public class Route
{
    public string MethodType { get; set; }

    public string Uri { get; set; }

    public bool RequireAuth;

    public Action<HttpListenerContext> Execute;

    public Route(string route, string type, bool authRequired,Action<HttpListenerContext> func)
    {
        Uri = route;
        RequireAuth = authRequired;
        Execute = func;
        MethodType = type;
    }

}
