namespace Framework.Networking.HTTPComponents.Enums;

public enum HttpStatusCode
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
