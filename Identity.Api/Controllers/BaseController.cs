using Microsoft.AspNetCore.Mvc;
using Identity.Contracts.DTOs;
using Identity.Domain.Exceptions;
namespace Identity.Api.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleException(Exception ex, string controllerName)
        {
            switch (ex)
            {
                case KeyNotFoundException _:
                    return NotFound(ApiResponse<string>.NotFoundResponse($"Error at the {controllerName}: {ex.Message}"));

                case UnauthorizedAccessException _:
                    return Unauthorized(ApiResponse<string>.UnauthorizedResponse($"Error at the {controllerName}: {ex.Message}"));

                case ArgumentException _:
                    return BadRequest(ApiResponse<string>.BadRequestResponse($"Error at the {controllerName}: {ex.Message}"));

                case InvalidOperationException _:
                    return StatusCode(500, ApiResponse<string>.InternalErrorResponse($"Error at the {controllerName}: {ex.Message}"));

                case CustomExceptions.NoDataFoundException _:
                    return NotFound(ApiResponse<string>.NotFoundResponse($"Error at the {controllerName}: {ex.Message}"));

                default:
                    return StatusCode(500, ApiResponse<string>.InternalErrorResponse($"Error at the {controllerName}: {ex.Message}"));
            }
        }
    }
}