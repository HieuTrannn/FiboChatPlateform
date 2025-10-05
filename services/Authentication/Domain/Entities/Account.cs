using Authentication.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Authentication.Domain.Entities
{
    public class Account : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Firstname { get; set; }

        [Required]
        [MaxLength(100)]
        public string Lastname { get; set; }

        [MaxLength(255)]
        public string Password { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender Sex { get; set; }
        [Required]
        public string Email { get; set; }

        [MaxLength(10)]
        public string PhoneNumber { get; set; }

        public string? Address { get; set; }

        public RoleType Role { get; set; }

        [MaxLength(128)] public string? RefreshToken { get; set; }

        [MaxLength(128)] public DateTime? RefreshTokenExpiryTime { get; set; }

        public bool IsEmailVerified { get; set; }

        public AccountStatus AccountStatus { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
