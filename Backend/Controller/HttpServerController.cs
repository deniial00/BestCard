using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Logic.Controller;

class HttpServerController
{
	private HttpListener _httpListener;
	HttpServerController()
	{
		try
		{
            _httpListener = new();
		} 
		catch(SocketException ex)
		{
			//log
			Console.WriteLine($"Error at creating Socket\nError: {ex.Message}");
		}
	}

	public async Task Listen()
	{
		_httpListener.Start();

		while (_httpListener.IsListening)
		{
			HttpListenerContext context = await _httpListener.GetContextAsync();
			HttpListenerRequest request = context.Request;

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
