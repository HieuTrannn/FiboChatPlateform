using Authentication.Application.DTOs;
using Authentication.Application.Interfaces;
using Authentication.Domain.Abstraction;
using Authentication.Domain.Entities;
using Contracts.Common;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog.Core;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using static Authentication.Application.DTOs.AuthenDTO;

namespace Authentication.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<IUserService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRsaService _rsaService;
        //private readonly ICacheService _cacheService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ILogger<IUserService> logger, IUnitOfWork unitOfWork, IRsaService rsaService, IEmailService emailService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _rsaService = rsaService;
            _emailService = emailService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<RegisterResponse>> RegisterUserAsync(RegisterReq request)
        {
            try
            {
                var userRepo = _unitOfWork.GetRepository<Account>();
                var existingUser = await userRepo.FindByConditionAsync(u => u.Email == request.Email);

                if (existingUser != null)
                    return ApiResponse<RegisterResponse>.BadRequest("Email already exists");

                var existingStudent = await userRepo.FindByConditionAsync(u => u.StudentID == request.StudentID);

                if (existingStudent != null)
                    return ApiResponse<RegisterResponse>.BadRequest("StudentID already exists");

                // Tạo tài khoản
                var rawPassword = GenerateRandomPassword();
                var account = request.Adapt<Account>();
                account.Password = _rsaService.Encrypt(rawPassword);
                account.CreatedAt = DateTime.UtcNow;
                account.IsVerified = false;

                var roleRepository = _unitOfWork.GetRepository<Role>();
                var userRole = await roleRepository.FindByConditionAsync(
                    r => r.RoleName.ToLower() == StaticEnum.RoleEnum.Student.ToString().ToLower());

                if (userRole == null)
                {
                    _logger.LogError("User role not found in database");
                    return ApiResponse<RegisterResponse>.BadRequest("Registration failed", new RegisterResponse
                    {
                        Success = false,
                        Message = "Error assigning user role"
                    });
                }

                account.RoleId = userRole.Id;

                await userRepo.InsertAsync(account);
                await _unitOfWork.SaveChangeAsync();

                await SendUserRegistrationEmail(account, rawPassword);

                return ApiResponse<RegisterResponse>.Ok("Registration successful", new RegisterResponse
                {
                    Success = true,
                    Message = "Please check your email to verify your account."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering user");
                return ApiResponse<RegisterResponse>.InternalError("An unexpected error occurred while registering user: " + ex);
            }
        }

        public async Task<AuthResponse> Login(AuthRequest request)
        {
            try
            {
                var accountRepo = _unitOfWork.GetRepository<Account>();
                var account = await accountRepo.FindByConditionAsync(u => u.Email == request.Email);
                if (account == null)
                {
                    _logger.LogInformation("Login failed: Account not found");
                    throw new KeyNotFoundException("You have not register");
                }

                if (!VerifyPassword(request.Password, account.Password))
                {
                    _logger.LogInformation("Login failed: Invalid username or password");
                    throw new KeyNotFoundException("Invalid username or password");
                }

                //Get the role information
                //var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(account.Id ?? string.Empty);
                //account.Role = role;

                var token = GenerateJwtToken(account);
                var response = request.Adapt<AuthResponse>();
                response.Token = (string?)token;

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError("Error at login: {Message}", e.Message);
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public async Task<RegisterResponse> ChangePassword(string email, ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                _logger.LogInformation("Changing password for user");
                var userRepo = _unitOfWork.GetRepository<Account>();
                var user = await userRepo.FindByConditionAsync(u => u.Email == email);
                if (user == null)
                {
                    _logger.LogWarning("User not found with email: {email}", email);
                    return new RegisterResponse { Success = false, Message = "User not found" };
                }
                if (!VerifyPassword(changePasswordRequest.OldPassword, user.Password))
                {
                    _logger.LogWarning("Old password is incorrect for user: {email}", email);
                    return new RegisterResponse { Success = false, Message = "Old password is incorrect" };
                }
                if (changePasswordRequest.NewPassword != changePasswordRequest.ConfirmNewPassword)
                {
                    _logger.LogWarning("New password and confirm password do not match for user: {email}", email);
                    return new RegisterResponse { Success = false, Message = "New password and confirm password do not match" };
                }
                user.Password = _rsaService.Encrypt(changePasswordRequest.NewPassword);
                await userRepo.UpdateAsync(user);
                await _unitOfWork.SaveChangeAsync();

                return new RegisterResponse { Success = true, Message = "Password changed successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return new RegisterResponse { Success = false, Message = ex.Message };
            }
            ;
        }

        public async Task<RegisterResponse> ForgotPasswordAsync(string email)
        {
            var userRepository = _unitOfWork.GetRepository<Account>();
            var user = await userRepository.FindByConditionAsync(u => u.Email == email);

            if (user == null)
            {
                _logger.LogWarning("Forgot password requested for non-existent email: {Email}", email);
                return new RegisterResponse
                {
                    Success = true,
                    Message = "if your account exists, you will recive the email"
                };
            }

            string resetToken = GenerateResetToken(email);
            var resetData = new PasswordResetData
            {
                Token = resetToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            await SendPasswordResetEmail(user, resetData.Token);

            return new RegisterResponse
            {
                Success = true,
                Message = "Check your email to reset your password."
            };
        }

        public async Task<RegisterResponse> ResetPasswordAsync(string token, string newPassword)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                }, out _);

                var email = principal.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

                if (email == null)
                    return new RegisterResponse { Success = false, Message = "Token invalidate" };

                var userRepository = _unitOfWork.GetRepository<Account>();
                var user = await userRepository.FindByConditionAsync(u => u.Email == email);

                if (user == null)
                    return new RegisterResponse { Success = false, Message = "User not found" };

                user.Password = _rsaService.Encrypt(newPassword);
                user.UpdatedAt = DateTime.UtcNow;
                await userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangeAsync();

                return new RegisterResponse { Success = true, Message = "Change Password Succcessfully" };
            }
            catch (SecurityTokenExpiredException)
            {
                return new RegisterResponse { Success = false, Message = "Token Exprired" };
            }
            catch
            {
                return new RegisterResponse { Success = false, Message = "Token invalid" };
            }
        }


        private async Task SendPasswordResetEmail(Account account, string token)
        {
            string resetLink = $"https://fiboaichat.com/reset-password?token={token}";

            var templateData = new Dictionary<string, string>
              {
                { "USERNAME", account.Email },
                { "RESET_LINK", resetLink },
                { "subject", "Đặt Lại Mật Khẩu - FiboAiChat" }
            };

            await _emailService.SendEmailAsync(
                recipientEmail: account.Email,
                recipientName: account.Email,
                templateFileName: "ResetPassword.html",
                templateData: templateData
            );
        }

        private bool VerifyPassword(string inputPassword, string encryptedPassword)
        {
            string decryptedPassword = _rsaService.Decrypt(encryptedPassword);
            return decryptedPassword != null && inputPassword == decryptedPassword;
        }
        private object GenerateJwtToken(Account account)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                    //new Claim(ClaimTypes.Role, account.Role.RoleName.ToString()),
                    //new Claim(ClaimTypes.Name, account.FullName.ToString()),
                    new Claim(ClaimTypes.Email, account.Email.ToString()),
                    // new Claim("userId", account.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
                }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    Issuer = "FIBO AI CHAT",
                    Audience = "FIBOAICHAT.API",
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error generating JWT token: {Message}", ex.Message);
                throw new InvalidOperationException("Failed to generate JWT token.", ex);
            }
        }

        private async Task SendUserRegistrationEmail(Account account, string rawPassword)
        {
            var templateData = new Dictionary<string, string>
            {
                { "USERNAME", account.Email },
                { "PASSWORD", rawPassword },
                { "subject", "Thông tin đăng nhập FiboAiChat"}
            };

            await _emailService.SendEmailAsync(
                recipientEmail: account.Email,
                recipientName: account.Email,
                templateFileName: "UserRegistration.html",
                templateData: templateData
            );
        }

        private static string GenerateRandomPassword(int length = 10)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            var random = new Random();
            var chars = Enumerable.Range(0, length)
                .Select(x => validChars[random.Next(validChars.Length)]);
            return new string(chars.ToArray());
        }

        private string GenerateResetToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("email", email),
                new Claim("purpose", "password_reset")
                 }),
                Expires = DateTime.UtcNow.AddHours(1), // 1 giờ
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
