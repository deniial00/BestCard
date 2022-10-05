using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Logic.Controller;

class HttpServerController
{
    private HttpListener _httpListener;
    public HttpServerController()
    {
        Console.Write("Server starting ...");
        try
        {
            _httpListener = new();
            _httpListener.Prefixes.Add("http://+:80/");
        }
        catch (HttpListenerException ex)
        {
            //log
            Console.Write("Error at creating Socket\nError: {ex.Message}");
        }
        Console.Write(" OK!\n");
    }

    public async Task Listen()
    {
        _httpListener.Start();

        while (_httpListener.IsListening)
        {
            Console.WriteLine("listening\n");
            HttpListenerContext context = await _httpListener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            Console.WriteLine(request.ToString());

            Uri? urlString = request.Url;
            HttpListenerResponse response = context.Response;
        }
    }

    public void Stop()
    {
        _httpListener.Stop();
    }

    public async Task HandleRequest(Task<Socket> sock)
    {

        return;
    }
}