using Framework.Networking.Models.Enums;

namespace Framework.Networking.Models.Interfaces;

public interface IHttpResponseHeader : IHttpHeader
{
    HttpMessageType StatusCode { get; }
}