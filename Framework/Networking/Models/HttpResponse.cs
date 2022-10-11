using Framework.Networking.Models.Enums;

namespace Framework.Networking.Models;
class HttpResponse
{
    public HttpResponseHeader Header { get; }

    public HttpBody? Body { get; private set; }

    public HttpMessageType MessageType { get; }

    public HttpResponse(string utf8Data)
    {
        List<string> parsedString = utf8Data.Split("\r\n\r\n").ToList();

        Header = new HttpResponseHeader(parsedString[0]);
        if (parsedString[1].Length > 1)
            Body = new HttpBody(parsedString[1], Header.Headers["Content-Type"]);
    }


    public HttpResponse()
    {
        MessageType = HttpMessageType.Response;
        Header = new HttpResponseHeader("1.1", false);
        Header.AddHeader("Content-Type", "application/json");
    }

    public override string ToString()
    {
        string header = Header.ToString();
        string? body = null;
        if (Body != null)
            body = Body.ToString();

        return header + "\r\n" + body;
    }

    public void SetStatusCode(HttpStatusCode statusCode)
    {
        Header.StatusCode = statusCode;
    }

    public void AddBody(HttpBody body)
    {
        Body = body;
    }
}
