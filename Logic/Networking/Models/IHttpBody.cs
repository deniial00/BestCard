namespace Logic.Networking.Models;

interface IHttpBody
{
    public string ContentType { get; set; }
    public string Content { get;  set; }

    public bool AddContent(string utf8data);

    public string ToString();
}
