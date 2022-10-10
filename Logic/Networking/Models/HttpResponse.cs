namespace Logic.Networking.Models;
class HttpResponse
{
    public HTTPMessageType Type { get; private set; }
    public HttpResponseHeader Header { get; }

    //public HTTPBody Body { get;  }

    public HttpResponse(string utf8Data, HTTPMessageType type)
    {
        Type = type;
        if (Type == HTTPMessageType.Response)
            Header = new HttpResponseHeader(utf8Data);
        else
            throw new ArgumentException("Wrong Message Type");
    }
}
