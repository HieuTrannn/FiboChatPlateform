using Identity.Infrastructure.Interfaces;
using Identity.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Identity.Contracts.DTOs.UserDtos;
using Identity.Domain.Exceptions;

namespace Identity.Application.Implements
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<UserResponse>> GetAllUsersAsync()
        {            
            var users = await _unitOfWork.Users.GetAllAsync();
            var response = users.Select(user => new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                Status = user.Status,
                Cohort = user.Cohort,
                CreatedAt = user.CreatedAt,
            }).ToList();
            
            _logger.LogInformation("Mapped {Count} users to response", response.Count);
            return response;
        }

        public async Task<UserResponse> GetUserByIdAsync(Guid id) 
        {            
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogError("User not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("User not found");
            }

            var response = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                Status = user.Status,
                Cohort = user.Cohort,
                CreatedAt = user.CreatedAt,
            };

            return response;
        }
    }
}