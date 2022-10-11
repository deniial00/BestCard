using Framework.Networking.HTTPComponents;
using Framework.Networking.HTTPComponents.Enums;

namespace Framework.Networking.Models;

class Route
{
    public HttpMethodType MethodType { get; set; }

    public string Uri { get; set; }

    public Func<HttpRequest, HttpResponse> Function { get; set; }

    public Route(string route, HttpMethodType type, Func<HttpRequest, HttpResponse> func)
    {
        Uri = route;
        Function = func;
        MethodType = type;
    }
}
