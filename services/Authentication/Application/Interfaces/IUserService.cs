using Authentication.Application.DTOs;
using Contracts.Common;
using static Authentication.Application.DTOs.AuthenDTO;

namespace Authentication.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<RegisterResponse>> RegisterUserAsync(RegisterReq request);
        Task<AuthResponse> Login(AuthRequest request);
        Task<RegisterResponse> ChangePassword(string email, ChangePasswordRequest changePasswordRequest);
    }
}
