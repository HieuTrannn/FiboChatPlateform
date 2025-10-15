using static Course.Domain.Enums.StaticEnums;

namespace Course.Domain.DTOs.SemesterDTOs
{
    public class SemesterUpdateRequest
    {
        public string? Code { get; set; } = null;
        public SemesterTerm? Term { get; set; }
        public int? Year { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}