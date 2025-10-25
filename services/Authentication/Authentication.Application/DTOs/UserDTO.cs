
using Authentication.Domain.Enum;

namespace Authentication.Application.DTOs
{
    public class UserDTO
    {
        public class UserInfo
        {
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string Email { get; set; }
            public string? PhoneNumber { get; set; }
            public string? StudentID { get; set; }
        }

        public class UserResponse
        {
            public Guid Id { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string Email { get; set; }
            public string? PhoneNumber { get; set; }
            public string? StudentID { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public Gender? Sex { get; set; }
            public string? Address { get; set; }
            public bool IsVerified { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class UpdateUserRequest
        {
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string PhoneNumber { get; set; }
            public DateOnly DateOfBirth { get; set; }
            public Gender Sex { get; set; }
            public string Address { get; set; }
        }
    }
}
