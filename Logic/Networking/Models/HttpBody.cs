namespace Logic.Networking.Models;

class HttpBody : IHttpBody
{
    public string ContentType { get; set; }
    public string Content { get; set; }

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
