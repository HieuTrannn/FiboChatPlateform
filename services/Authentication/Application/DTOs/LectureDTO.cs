namespace Authentication.Application.DTOs
{
    public class LectureDTO
    {
        public class LectureRequest
        {
            public string FullName { get; set; }
            public string Email { get; set; }
        }

        public class LectureResponse
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string? Gender { get; set; }
        }
    }
}
