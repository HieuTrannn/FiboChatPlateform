using static Course.Domain.Enums.StaticEnums;

namespace Course.Domain.DTOs.SemesterDTOs
{
    public class SemesterResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public SemesterTerm Term { get; set; }
        public int Year { get; set; }
        public SemesterStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}