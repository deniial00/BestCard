
namespace Logic.Networking.Models;

class Route
{
    public HTTPMethodType MethodType { get; set; }

    public string Url { get; set; }

    public Func<HttpRequest, HttpResponse> Function { get; set; }

    public Route(HTTPMethodType type, Func<HttpRequest,HttpResponse> func)
    {
        Function = func;
        MethodType = type;
    }
}
