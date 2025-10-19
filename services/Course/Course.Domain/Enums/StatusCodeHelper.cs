using System.ComponentModel;

namespace Course.Domain.Enums
{
    public enum StatusCodeHelper
    {
        [Description("Success")] OK = 200,
        [Description("Created")] Created = 201,
        [Description("No Content")] NoContent = 204,
        [Description("Bad Request")] BadRequest = 400,
        [Description("Unauthorized")] Unauthorized = 401,
        [Description("Forbidden")] Forbidden = 403,
        [Description("Not Found")] NotFound = 404,
        [Description("Method Not Allowed")] MethodNotAllowed = 405,
        [Description("Not Acceptable")] NotAcceptable = 406,
        [Description("Request Timeout")] RequestTimeout = 408,
        [Description("Conflict")] Conflict = 409,
        [Description("Precondition Failed")] PreconditionFailed = 412,
        [Description("Unprocessable Entity")] UnprocessableEntity = 422,
        [Description("Precondition Required")] PreconditionRequired = 428,
        [Description("Too Many Requests")] TooManyRequests = 429,
        [Description("Request Header Fields Too Large")] RequestHeaderFieldsTooLarge = 431,
        [Description("Unavailable For Legal Reasons")] UnavailableForLegalReasons = 451,
        [Description("Client Closed Request")] ClientClosedRequest = 499,
        [Description("Internal Server Error")] ServerError = 500,
        [Description("Not Implemented")] NotImplemented = 501,
        [Description("Bad Gateway")] BadGateway = 502,
        [Description("Service Unavailable")] ServiceUnavailable = 503,
        [Description("Gateway Timeout")] GatewayTimeout = 504,
        [Description("HTTP Version Not Supported")] HttpVersionNotSupported = 505,
        [Description("Insufficient Storage")] InsufficientStorage = 507,
        [Description("Loop Detected")] LoopDetected = 508,
        [Description("Not Extended")] NotExtended = 510,
        [Description("Network Authentication Required")] NetworkAuthenticationRequired = 511,
        [Description("Network Connect Timeout Error")] NetworkConnectTimeoutError = 599,
    }
}