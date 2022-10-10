using Logic.Networking;
using Logic.Networking.Models;


string data = "HTTP/1.1 200 OK\r\nDate: Sun, 18 Oct 2009 08:56:53 GMT\r\nServer: Apache/2.2.14 (Win32)\r\nLast-Modified: Sat, 20 Nov 2004 07:16:26 GMT\r\nETag: \"10000000565a5-2c-3e94b66c2e680\"\r\nAccept-Ranges: bytes\r\nContent-Length: 44\r\nConnection: close\r\nContent-Type: text/html\r\nX-Pad: avoid browser bug\r\n\r\n<html><body><h1>It works!</h1></body></html>\r\n";

HttpResponse response = new(data);

if (response != null)
    Console.WriteLine("yes");

//try
//{
//    var server = new ServerController();
//    await server.Listen();
//}
//catch(Exception ex)
//{
//    Console.WriteLine(ex);
//}