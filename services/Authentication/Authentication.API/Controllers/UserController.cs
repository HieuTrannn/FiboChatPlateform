using Authentication.Application.DTOs;
using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Authentication.Application.DTOs.AuthenDTO;
using static Authentication.Application.DTOs.UserDTO;

namespace Authentication.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public UserController(ILogger<UserController> logger, IUserService userService, IConfiguration configuration)
        {
            _logger = logger;
            _userService = userService;
            _configuration = configuration;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterReq request)
        {
            try
            {
                var response = await _userService.RegisterUserAsync(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError("{Classname} - Error at register async cause by {}", nameof(UserController), ex.Message);
                return BadRequest(ex);
            }

        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] AuthRequest request)
        {
            try
            {
                var response = await _userService.Login(request);
                return Ok(ApiResponse<AuthResponse>.Ok(response, "Login successful", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError("{Classname} - Error at get account async cause by {}", nameof(UserController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError("Error: " + ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost("login-google")]
        public async Task<IActionResult> LoginWithGoogleAsync([FromForm] string idToken)
        {
            try
            {
                var response = await _userService.LoginWithGoogleAsync(idToken);
                return Ok(ApiResponse<AuthResponse>.Ok(response, "Login with Google successful", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError("{Classname} - Error at login with Google async cause by {}", nameof(UserController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError("Error: " + ex.Message));
            }
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordRequest request)
        {
            try
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("No email claim found in JWT token");
                    return Unauthorized(new RegisterResponse { Success = false, Message = "Invalid token: Email claim missing" });
                }

                var response = await _userService.ChangePasswordAsync(email, request);
                if (!response.Success)
                {
                    return BadRequest(ApiResponse<RegisterResponse>.BadRequest(response.Message));
                }

                return Ok(ApiResponse<Account>.OkResponse(response.Message, StatusCode: "Success"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.InternalError("Error at the " + nameof(UserController) + ": " + ex.Message));
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromForm] string email)
        {
            try
            {
                var response = await _userService.ForgotPasswordAsync(email);
                if (!response.Success)
                {
                    return BadRequest(ApiResponse<RegisterResponse>.BadRequestResponse(response.Message));
                }
                return Ok(ApiResponse<Account>.OkResponse(response.Message, StatusCode: "Success"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.InternalError("Error in forgot password: " + ex.Message));
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordRequest request)
        {
            try
            {
                var response = await _userService.ResetPasswordAsync(request);
                if (!response.Success)
                {
                    return BadRequest(ApiResponse<RegisterResponse>.BadRequestResponse(response.Message));
                }
                return Ok(ApiResponse<Account>.OkResponse(response.Message, StatusCode: "Success"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.InternalError("Error in reset password: " + ex.Message));
            }
        }

        
        [Authorize]
        [HttpPut("change-password-first-time")]
        public async Task<IActionResult> ChangePasswordFirstTime([FromForm] ChangePasswordFirstTimeRequest request)
        {
            try
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("No email claim found in JWT token");
                    return Unauthorized(new RegisterResponse { Success = false, Message = "Invalid token: Email claim missing" });
                }
                var response = await _userService.ChangePasswordFirstTimeAsync(request);
                if (!response.Success)
                {
                    return BadRequest(ApiResponse<RegisterResponse>.BadRequest(response.Message));
                }
                return Ok(ApiResponse<Account>.OkResponse(response.Message, StatusCode: "Success"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.InternalError("Error at the " + nameof(UserController) + ": " + ex.Message));
            }
        }

        [Authorize]
        [HttpGet("get-user-profile")]
        public async Task<IActionResult> GetUserProfileAsync()
        {
            try
            {
                var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    _logger.LogWarning("No name identifier claim found in JWT token");
                    return Unauthorized(new RegisterResponse { Success = false, Message = "Invalid token: Name identifier claim missing" });
                }
                var response = await _userService.GetUserProfileAsync(currentUserId);
                return Ok(ApiResponse<UserInfo>.Ok(response, "Get user profile successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(UserController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(UserController)}: {ex.Message}"));
            }
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateUserProfileAsync([FromForm] UserInfo userInfo)
        {
            try
            {
                var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    _logger.LogWarning("No name identifier claim found in JWT token");
                    return Unauthorized(new RegisterResponse { Success = false, Message = "Invalid token: Name identifier claim missing" });
                }
                var user = await _userService.GetUserProfileAsync(currentUserId);
                if (user == null)
                {
                    return NotFound(ApiResponse<UserInfo>.NotFound("User not found"));
                }
                var response = await _userService.UpdateUserProfileAsync(currentUserId, userInfo);
                return Ok(ApiResponse<UserInfo>.Ok(response, "Update user profile successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(UserController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(UserController)}: {ex.Message}"));
            }
        }

        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsersAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _userService.GetAllUsersAsync(page, pageSize);
                return Ok(ApiResponse<BasePaginatedList<UserResponse>>.Ok(response, "Get all users successfully", "200"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.InternalError("Error at the " + nameof(UserController) + ": " + ex.Message));
            }
        }

        [HttpGet("get-user-by-id")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] string userId)
        {
            try
            {
                var response = await _userService.GetUserByIdAsync(userId);
                return Ok(ApiResponse<UserResponse>.Ok(response, "Get user by id successfully", "200"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.InternalError("Error at the " + nameof(UserController) + ": " + ex.Message));
            }
        }

        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] string userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(ApiResponse<string>.NotFound("User not found"));
                }
                await _userService.DeleteUserAsync(userId);
                return Ok(ApiResponse<string>.OkResponse("Delete user successfully", StatusCode: "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(UserController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(UserController)}: {ex.Message}"));
            }
        }

    }
}

