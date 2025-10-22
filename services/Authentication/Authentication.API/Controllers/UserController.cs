using Authentication.Application.DTOs;
using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Authentication.Application.DTOs.AuthenDTO;
using static Authentication.Application.DTOs.UserDTO;

namespace Authentication.API.Controllers
{
    [Route("api/user")]
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
        [HttpPost("login-gg")]
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

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserById([FromForm] string userId)
        {
            try
            {
                var user = await _userService.GetUserById(userId);
                if (user == null)
                {
                    return NotFound(ApiResponse<UserInfo>.NotFound("User not found"));
                }
                return Ok(ApiResponse<UserInfo>.Ok(user, "Get user by id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(UserController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(UserController)}: {ex.Message}"));
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
    }
}

