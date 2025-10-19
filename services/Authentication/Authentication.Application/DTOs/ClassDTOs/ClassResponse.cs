using Authentication.Domain.Enum;

namespace Authentication.Application.DTOs.ClassDTOs
{
    public class ClassResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Status { get; set; } = "active";    // active | disabled
        public DateTime CreatedAt { get; set; }
        public LecturerResponse? Lecturer { get; set; }
        public SemesterResponse? Semester { get; set; }
    }

    public class LecturerResponse
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
    }

    public class SemesterResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public SemesterTermEnum Term { get; set; }
        public int Year { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}