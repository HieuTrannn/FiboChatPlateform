using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Contracts.DTOs.UserDtos
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string? Cohort { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}