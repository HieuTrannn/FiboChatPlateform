using System.ComponentModel.DataAnnotations;

namespace Authentication.Application.DTOs
{
    public class AuthenDTO
    {
        public class RegisterReq
        {
            [Required(ErrorMessage = "Email is required")]
            [MaxLength(255, ErrorMessage = "Email must not exceed 255 characters")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            public string Email { get; set; }
            [Required]
            [MaxLength(255)]
            public string Firstname { get; set; }
            [Required]
            [StringLength(255)]
            public string Lastname { get; set; }
            [Required]
            [MaxLength(255)]
            public string StudentID { get; set; }

        }

        public class RegisterResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        public class AuthRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class AuthResponse
        {
            public string? Token { get; set; }
            public bool Success { get; set; }
            public string? Message { get; set; }
        }

        public class GoogleUserInfo
        {
            public string Uid { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Name { get; set; }
            public string? Picture { get; set; }
        }
    }


}
