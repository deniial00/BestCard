using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Logic.Controller;

class HttpServerController
{
	private Socket _tcpListener;
	private bool _isRunning;
	HttpServerController(string ip = "127.0.0.1", int port = 8000)
	{
		try
		{
			IPEndPoint ipEndPoint= new(IPAddress.Parse(ip),port);
			var listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream,ProtocolType.Tcp);
			listener.Bind(ipEndPoint);    
		} 
		catch(SocketException ex)
		{
			//log
			Console.WriteLine($"Error at creating Socket\nError: {ex.Message}");
		}
	}

	public async Task Listen()
	{
		while (_isRunning)
		{
			var clientSocket = await _tcpListener.AcceptAsync();

			while (true)
			{
				var buffer = new byte[1024];
				var receivedBytes = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
				string response = Encoding.UTF8.GetString(buffer, 0, receivedBytes);

				// End of Line marker
				string eom = "<|EOM|>";

				if(response.IndexOf(eom) > -1)
				{
					break;
				}
			}
			//new Thread(() => HandleRequest(await clientSocket)).Start();
		}
	}

	public void Stop() 
	{
		_isRunning = false;
	}

	public async Task HandleRequest(Task<Socket> sock)
	{

		return;
	}
}
