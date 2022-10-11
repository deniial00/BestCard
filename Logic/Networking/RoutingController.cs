using Logic.Networking.Models;

namespace Logic.Networking;

 class RoutingController
{
    private Dictionary<string,Route> Routes { get; set; }

    public RoutingController()
    {
        Routes = new Dictionary<string, Route>();
    }

    public void AddRoute(string route, HTTPMethodType type, Func<HttpRequest, HttpResponse> func)
    {
        var routeNode = new Route(type, func);
        Routes.Add("route", routeNode);
    }
}
