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
            public string? Message { get; set; }

            [JsonPropertyOrder(4)]
            public T? Data { get; set; }
        public ApiResponse() { }

        public ApiResponse(int statusCode, string code,  string? message, T? data)
        {
            StatusCode = statusCode;
            Code = code;
            Message = message;
            Data = data;
        }
    }

}
