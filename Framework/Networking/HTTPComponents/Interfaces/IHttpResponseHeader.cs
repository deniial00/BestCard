using Framework.Networking.HTTPComponents.Enums;

namespace Framework.Networking.HTTPComponents.Interfaces;

public interface IHttpResponseHeader : IHttpHeader
{
    HttpStatusCode StatusCode { get; set; }
}