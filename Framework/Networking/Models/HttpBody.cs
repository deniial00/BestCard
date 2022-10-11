using Framework.Networking.Models.Interfaces;

namespace Framework.Networking.Models;

class HttpBody : IHttpBody
{
    public string ContentType { get; private set; }
    public string Content { private get; set; }

    public HttpBody(string content, string contentType)
    {
        ContentType = contentType;
        Content = content;
    }

    public bool AddContent(string utf8data)
    {
        Content = utf8data;
        return true;
    }

    public override string ToString()
    {
        return Content;
    }
}
