using Framework.Networking.Controller;
using Framework.Networking.Models;
using Framework.Networking;
using Framework.Networking.HTTPComponents;

//string data = "HTTP/1.1 200 OK\r\nDate: Sun, 18 Oct 2009 08:56:53 GMT\r\nServer: Apache/2.2.14 (Win32)\r\nLast-Modified: Sat, 20 Nov 2004 07:16:26 GMT\r\nETag: \"10000000565a5-2c-3e94b66c2e680\"\r\nAccept-Ranges: bytes\r\nContent-Length: 44\r\nConnection: close\r\nContent-Type: application/json\r\n\r\n\r\n";

//HttpResponse res = new(data);

//if (res != null)
//    Console.WriteLine(res.ToBytes());

try
{
    var server = new ServerController();
    server.Start();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}