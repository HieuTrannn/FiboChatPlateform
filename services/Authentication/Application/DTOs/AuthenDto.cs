using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //public string FullName { get; set; }
            public string? Token { get; set; }
            public string? Message { get; set; }
            public bool RequirePasswordChange { get; set; } = false;
        }
    }
}
