using static Course.Domain.Enums.StaticEnums;


namespace Course.Domain.Entities
{
    public class Class
    {
        public Guid Id { get; set; }
        public Guid SemesterId { get; set; }
        public string Code { get; set; } = null!;
        public ClassStatus Status { get; set; } = ClassStatus.Pending;    // active | disabled | pending
        public Guid? LecturerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Semester Semester { get; set; } = null!;
        public ICollection<ClassEnrollment> Enrollments { get; set; } = new List<ClassEnrollment>();
    }
}
