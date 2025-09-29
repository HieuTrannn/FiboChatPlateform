using Identity.Contracts.DTOs.UserDtos;

namespace Identity.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetAllUsersAsync();
        Task<UserResponse> GetUserByIdAsync(Guid id);
    }
}