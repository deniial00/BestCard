using Framework.Networking.HTTPComponents;
using Framework.Networking.HTTPComponents.Enums;
using Framework.Networking.Models;

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
        var routeNode = new Route(route, type, func);
        Routes.Add(route, routeNode);
    }
}
