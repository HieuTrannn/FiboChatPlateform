namespace Authentication.Application.DTOs.ClassDTOs
{
    public class ClassStudentResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Status { get; set; } = "active";    // active | disabled
        public DateTime CreatedAt { get; set; }
        public ClassLecturerResponse Lecturer { get; set; }
        public List<StudentResponse> Students { get; set; } = new List<StudentResponse>();
    }

    public class ClassLecturerResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
    }

    public class StudentResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string StudentId { get; set; } = null!;
        public string RoleInClass { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}