namespace Framework.Networking.Models;
class HttpRequest
{
    public HttpRequestHeader Header { get; }

    public HttpRequest(string utf8Data)
    {
        Header = new HttpRequestHeader(utf8Data);
    }

    public override string ToString()
    {
        string requestString = Header.ToString();

        // add extra line to indicate end of header
        requestString += "\r\n";

        return requestString;
    }
}
