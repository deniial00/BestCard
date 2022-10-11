using System.Text;

namespace Framework.Networking.HTTPComponents;
class HttpRequest
{
    public HttpRequestHeader Header { get; }

    public HttpRequest(string utf8Data)
    {
        Header = new HttpRequestHeader(utf8Data);
    }

    public HttpRequest(byte[] buffer, int bytesRead)
    {
        string utf8Data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Header = new HttpRequestHeader(utf8Data);
    }


    public override string ToString()
    {
        string requestString = Header.ToString();

        // add extra line to indicate end of header
        requestString += "\r\n";

        return requestString;
    }

    public byte[] ToBytes()
    {
        return Encoding.UTF8.GetBytes(ToString());
    }

}
