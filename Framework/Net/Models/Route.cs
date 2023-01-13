using System.Net;

namespace Framework.Net.Models;

public class Route
{
    public HttpMethod MethodType { get; set; }

    public string Uri { get; set; }

    public bool RequireAuth;

    public Action<HttpListenerRequest, HttpListenerResponse> Execute;

    public Route(string route, HttpMethod type, bool authRequired,Action<HttpListenerRequest, HttpListenerResponse> func)
    {
        Uri = route;
        RequireAuth = authRequired;
        Execute = func;
        MethodType = type;
    }

}
