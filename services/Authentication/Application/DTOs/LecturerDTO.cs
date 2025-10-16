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
            public string FullName { get; set; }
            public string Email { get; set; }
            public string? Gender { get; set; }
        }
    }
}
