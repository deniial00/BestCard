using Framework.Networking.Models.Enums;

namespace Framework.Networking.Models.Interfaces;

public interface IHttpRequestHeader : IHttpHeader
{
    HttpMethodType MethodType { get; }

    string Uri { get; }

}
