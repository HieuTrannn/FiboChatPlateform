using Authentication.Domain.Enum;

namespace Authentication.Application.DTOs.SemesterDTOs
{
    public class SemesterCreateRequest
    {
        public string Code { get; set; } = null!;
        public SemesterTermEnum Term { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}