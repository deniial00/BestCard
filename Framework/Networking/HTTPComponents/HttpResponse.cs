using System.Text;
using Framework.Networking.HTTPComponents.Enums;

namespace Framework.Networking.HTTPComponents;
class HttpResponse
{
    public HttpResponseHeader Header { get; }

    public HttpBody? Body { get; private set; }

    public HttpMessageType MessageType { get; }

    public HttpResponse(string utf8Data)
    {
        // split header and body
        List<string> parsedString = utf8Data.Split("\r\n\r\n").ToList();
        
        // set vars
        MessageType = HttpMessageType.Response;
        Header = new HttpResponseHeader(parsedString[0]);
        
        // check if body exits
        if (parsedString[1].Length > 1)
            Body = new HttpBody(parsedString[1], Header.Headers["Content-Type"]);
    }


    public HttpResponse()
    {
        MessageType = HttpMessageType.Response;
        Header = new HttpResponseHeader("1.1", false);
        Header.AddHeader("Content-Type", "application/json");
    }

    public HttpResponse(byte[] buffer, int bytesRead)
    {
        string utf8Data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Header = new HttpResponseHeader(utf8Data);
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

    public byte[] ToBytes()
    {
        return Encoding.UTF8.GetBytes(ToString());
    }

}
