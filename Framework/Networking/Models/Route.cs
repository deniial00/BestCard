
using Framework.Networking.Models.Interfaces;
using Framework.Networking.Models.Enums;

namespace Framework.Networking.Models;

class Route
{
    public HttpMethodType MethodType { get; set; }

    public string Url { get; set; }

    public Func<HttpRequest, HttpResponse> Function { get; set; }

    public Route(HttpMethodType type, Func<HttpRequest, HttpResponse> func)
    {
        Function = func;
        MethodType = type;
    }
}
