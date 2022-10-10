namespace Logic.Networking.Models;
class HttpResponse
{
    public HttpResponseHeader Header { get; }

    public HttpBody Body { get; }

    public HttpResponse(string utf8Data)
    {
        List<string> parsedString = utf8Data.Split("\r\n\r\n").ToList<string>();

        Header = new HttpResponseHeader(parsedString[0]);
        //Body = new HttpBody(parsedString[1], Header.Headers["Content-Type"]);
    }
}
