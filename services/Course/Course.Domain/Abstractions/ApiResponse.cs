using Course.Domain.Enums;

namespace Course.Contracts.DTOs
{
    public class ApiResponse<T>
    {
        public StatusCodeHelper Code { get; set; }

        public string StatusCode { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }

        public ApiResponse() { }

        public ApiResponse(StatusCodeHelper code, string statusCode, T? data, string? message)
        {
            StatusCode = statusCode;
            Code = code;
            Message = message;
            Data = data;
        }

        public ApiResponse(StatusCodeHelper code, string statusCode, string? message)
        {
            StatusCode = statusCode;
            Code = code;
            Message = message;
        }
        public static ApiResponse<T> OkResponse(T? data, string? mess, string StatusCode)
        {
            return new ApiResponse<T>(StatusCodeHelper.OK, StatusCodeHelper.OK.ToString(), data, mess);
        }
        public static ApiResponse<T> OkResponse(string? mess, string StatusCode)
        {
            return new ApiResponse<T>(StatusCodeHelper.OK, StatusCodeHelper.OK.ToString(), mess);
        }

        public static ApiResponse<T> CreateResponse(string? mess)
        {
            return new ApiResponse<T>(StatusCodeHelper.Created, StatusCodeHelper.Created.ToString(), mess);
        }

        public static ApiResponse<T> UnauthorizedResponse(string mess)
        {
            return new ApiResponse<T>(StatusCodeHelper.Unauthorized, StatusCodeHelper.Unauthorized.ToString(), mess);
        }

        public static ApiResponse<T> NotFoundResponse(string mess)
        {
            return new ApiResponse<T>(StatusCodeHelper.NotFound, StatusCodeHelper.NotFound.ToString(), mess);
        }

        public static ApiResponse<T> BadRequestResponse(string mess)
        {
            return new ApiResponse<T>(StatusCodeHelper.BadRequest, StatusCodeHelper.BadRequest.ToString(), mess);
        }

        public static ApiResponse<T> InternalErrorResponse(string mess)
        {
            return new ApiResponse<T>(StatusCodeHelper.ServerError, StatusCodeHelper.ServerError.ToString(), mess);
        }

        public static object ErrorResponse(string internalServerError, string exMessage)
        {
            return new
            {
                Status = "Error",
                Message = internalServerError,
                Details = exMessage
            };
        }
    }
}