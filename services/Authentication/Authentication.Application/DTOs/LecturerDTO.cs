using Authentication.Domain.Enum;

namespace Authentication.Application.DTOs
{
    public class LecturerDTO
    {
        public class LecturerRequest
        {
            public string FullName { get; set; }
            public string Email { get; set; }
        }

        public class LecturerResponse
        {
            public Guid LecturerId { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string? Gender { get; set; }
        }

        public class UpdateLecturerRequest
        {
            public string FullName { get; set; }
            public Gender Gender { get; set; }
        }
    }
}
