using System.ComponentModel.DataAnnotations;
using Authentication.Domain.Enum;

namespace Authentication.Domain.Entities
{
    public class Semester : BaseEntity
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;            // Ex: SP25, SU25
        public SemesterTermEnum Term { get; set; }          // Spring | Summer | Fall
        public int Year { get; set; }                        // 2024, 2025, ...
        public SemesterStatusEnum Status { get; set; } = SemesterStatusEnum.Pending;       // active | disabled | pending
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}
