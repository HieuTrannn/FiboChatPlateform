using System.Net;
using System.Text.Json.Serialization;

namespace Contracts.Common
{
    public class ApiResponse<T>
    {
        [JsonPropertyOrder(1)]
        public int StatusCode { get; set; }

        [JsonPropertyOrder(2)]
        public string Code { get; set; }

        [JsonPropertyOrder(3)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }

        [JsonPropertyOrder(4)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        public ApiResponse() { }

        public ApiResponse(int statusCode, string code, string? message = null, T? data = default)
        {
            StatusCode = statusCode;
            Code = code;
            Message = message;
            Data = data;
        }
        public static ApiResponse<T> Ok(string message, T? data = default)
            => new((int)HttpStatusCode.OK, nameof(HttpStatusCode.OK), message, data);
        public static ApiResponse<T> OkResponse(string? mess, string StatusCode)
        {
            return new ApiResponse<T>((int)HttpStatusCode.OK, StatusCodeHelper.OK.Name(), mess);
        }
        public static ApiResponse<T> BadRequest(string message, T? data = default)
            => new((int)HttpStatusCode.BadRequest, nameof(HttpStatusCode.BadRequest), message, data);

        public static ApiResponse<T> Error(string message)
            => new((int)HttpStatusCode.InternalServerError, nameof(HttpStatusCode.InternalServerError), message);

        public static ApiResponse<T> ValidationErrorResponse(T data)
           => new((int)HttpStatusCode.BadRequest, "ValidationError", "Invalid input data", data);
        public static ApiResponse<T> InternalError(string message)
       => new((int)HttpStatusCode.InternalServerError, HttpStatusCode.InternalServerError.ToString(), message);

        public static ApiResponse<T> BadRequestResponse(string mess)
        {
            return new ApiResponse<T>((int)HttpStatusCode.BadRequest, StatusCodeHelper.BadRequest.Name(), mess);
        }
    }
}
