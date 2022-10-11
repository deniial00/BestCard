using Framework.Networking.HTTPComponents.Enums;

namespace Framework.Networking.HTTPComponents.Interfaces;

public interface IHttpRequestHeader : IHttpHeader
{
    HttpMethodType MethodType { get; }

    string Uri { get; }

}
