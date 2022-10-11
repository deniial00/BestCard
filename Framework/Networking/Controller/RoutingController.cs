using Framework.Networking.Models;
using Framework.Networking.Models.Enums;

namespace Framework.Networking.Controller;

class RoutingController
{
    private Dictionary<string, Route> Routes { get; set; }

    public RoutingController()
    {
        Routes = new Dictionary<string, Route>();
    }

    public void AddRoute(string route, HttpMethodType type, Func<HttpRequest, HttpResponse> func)
    {
        var routeNode = new Route(type, func);
        Routes.Add("route", routeNode);
    }
}
