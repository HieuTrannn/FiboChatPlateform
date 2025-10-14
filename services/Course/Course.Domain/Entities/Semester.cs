using static Course.Domain.Enums.StaticEnums;

namespace Course.Domain.Entities
{
    public class Semester
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;            // Ex: SP25, SU25
        public SemesterTerm Term { get; set; }          // Spring | Summer | Fall
        public int Year { get; set; }                        // 2024, 2025, ...
        public SemesterStatus Status { get; set; } = SemesterStatus.Pending;       // active | disabled | pending
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Class> Classes { get; set; } = new List<Class>();
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    }
}
