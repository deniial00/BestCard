namespace Framework.Networking.HTTPComponents.Interfaces;

interface IHttpBody
{
    public string ContentType { get; }

    public string Content { set; }

    public bool AddContent(string utf8data);

    public string ToString();
}
