using Framework.Networking.Controller;
using Framework.Networking.Models;
using Framework.Networking;

//string data = "GET /docs/index.html HTTP/1.1\r\nHost: www.nowhere123.com\r\nAccept: image/gif, image/jpeg, */*\r\nAccept-Language: en-us\r\nAccept-Encoding: gzip, deflate\r\nUser-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)\r\n\r\n";

//HttpRequest req = new(data);

//if (req != null)
//    Console.WriteLine(req.ToString());

try
{
    var server = new ServerController();
    await server.Listen();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}