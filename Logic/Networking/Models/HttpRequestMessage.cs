namespace Logic.Networking.Models;
class HttpRequest
{
    public HTTPMessageType Type { get; private set; }
    public HttpRequestHeader Header { get; }

    //public HTTPBody Body { get;  }

    public HttpRequest(string utf8Data, HTTPMessageType type)
    {
        Type = type;
        if (Type == HTTPMessageType.Request)
            Header = new HttpRequestHeader(utf8Data);
        else
            throw new ArgumentException("Wrong Message Type");
    }
}
