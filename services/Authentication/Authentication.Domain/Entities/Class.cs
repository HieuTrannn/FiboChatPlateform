using Authentication.Domain.Enum;

namespace Authentication.Domain.Entities
{
    public class Class
    {
        public Guid Id { get; set; }
        public Guid SemesterId { get; set; }
        public string Code { get; set; } = null!;
        public ClassStatusEnum Status { get; set; } = ClassStatusEnum.Pending;    // active | disabled | pending
        public Guid? LecturerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Semester Semester { get; set; } = null!;
        public ICollection<ClassEnrollment> Enrollments { get; set; } = new List<ClassEnrollment>();
        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}
