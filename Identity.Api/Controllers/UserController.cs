using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Identity.Contracts.DTOs;
using Identity.Contracts.DTOs.UserDtos;

namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("identity/[controller]")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(ApiResponse<List<UserResponse>>.OkResponse(users, "Get all users successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(UserController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalErrorResponse($"Error at the {nameof(UserController)}: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                return Ok(ApiResponse<UserResponse>.OkResponse(user, "Get user by id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(UserController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalErrorResponse($"Error at the {nameof(UserController)}: {ex.Message}"));
            }
        }
    }
}