using Authentication.Application.DTOs;
using Contracts.Common;
using static Authentication.Application.DTOs.AuthenDTO;
using static Authentication.Application.DTOs.UserDTO;

namespace Authentication.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<RegisterResponse>> RegisterUserAsync(RegisterReq request);
        Task<AuthResponse> Login(AuthRequest request);
        Task<RegisterResponse> ChangePasswordAsync(string email, ChangePasswordRequest changePasswordRequest);
        Task<RegisterResponse> ForgotPasswordAsync(string email);
        Task<RegisterResponse> ResetPasswordAsync(ResetPasswordRequest request);
        Task<AuthResponse> LoginWithGoogleAsync(string idToken);
        Task<UserInfo> GetUserProfileAsync(string userId);
        Task<UserInfo> UpdateUserProfileAsync(string userId, UserInfo userInfo);
        Task DeleteUserAsync(string userId);
        Task<AuthResponse> ChangePasswordFirstTimeAsync(ChangePasswordFirstTimeRequest request);
        Task<UserResponse> GetUserByIdAsync(string userId);
        Task<BasePaginatedList<UserResponse>> GetAllUsersAsync(int page, int pageSize);
    }
}
