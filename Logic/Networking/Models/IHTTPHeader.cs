namespace Logic.Networking.Models;

public interface IHttpHeader
{
    Dictionary<string, string> Headers { get; }
    string HttpVersion { get; }
    HTTPMessageType MessageType { get; }
    HTTPMethodType MethodType { get; }
    HTTPStatusCode StatusCode { get; }

    void AddHeader(List<string> Header);
}

public enum HTTPMethodType
{
    GET,
    POST,
    PUT,
    DELETE
}

public enum HTTPStatusCode
{
    OK = 200,
    Created = 201,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    URITooLong = 414,
    InternalServerError = 500,
    NotImplemented = 501,
    HTTPVersionNotSupported = 505
}

public enum HTTPMessageType
{
    Response,
    Request
}