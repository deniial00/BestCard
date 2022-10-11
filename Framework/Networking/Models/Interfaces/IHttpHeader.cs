namespace Framework.Networking.Models.Interfaces;

public interface IHttpHeader
{
    Dictionary<string, string> Headers { get; }

    string HttpVersion { get; }

    void AddHeader(string key, string value);
}