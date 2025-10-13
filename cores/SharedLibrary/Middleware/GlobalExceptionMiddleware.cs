using Contracts.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Project.SharedLibrary.MiddleWare
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        private static readonly Dictionary<int, (string Title, string Message)> StatusMap = new()
        {
            { StatusCodes.Status400BadRequest, ("BadRequest", "Bad request. Please check your request and try again.") },
            { StatusCodes.Status401Unauthorized, ("Unauthorized", "Unauthorized access. Please login to continue.") },
            { StatusCodes.Status403Forbidden, ("Forbidden", "Forbidden access. You do not have permission.") },
            { StatusCodes.Status404NotFound, ("NotFound", "Resource not found. Please check the URL.") },
            { StatusCodes.Status429TooManyRequests, ("TooManyRequests", "Too many requests. Please try again later.") },
            { StatusCodes.Status500InternalServerError, ("InternalServerError", "Internal server error. Please try again later.") }
        };

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Nếu status code >=400 mà chưa có body => tự động trả lỗi chuẩn
                if (context.Response.StatusCode >= 400 && !context.Response.HasStarted)
                {
                    var (title, message) = StatusMap.TryGetValue(context.Response.StatusCode, out var value)
                        ? value
                        : ("Error", "Unexpected error occurred");

                    await WriteResponse(context, context.Response.StatusCode, title, message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception at {Path}", context.Request.Path);

                if (!context.Response.HasStarted)
                {
                    var (statusCode, title, message) = ex switch
                    {
                        TimeoutException => (StatusCodes.Status408RequestTimeout, "RequestTimeout", "Request timeout. Please try again later."),
                        TaskCanceledException => (StatusCodes.Status408RequestTimeout, "RequestTimeout", "Request was canceled."),
                        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized", "Unauthorized access."),
                        KeyNotFoundException => (StatusCodes.Status404NotFound, "NotFound", "Resource not found."),
                        _ => (StatusCodes.Status500InternalServerError, "InternalServerError", "An unexpected error occurred.")
                    };

                    await WriteResponse(context, statusCode, title, message);
                }
            }
        }

        private static async Task WriteResponse(HttpContext context, int statusCode, string code, string message)
        {
            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<string>(statusCode, code, message, null);
            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}
