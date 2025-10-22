using Contracts.Common;

namespace Course.Application.DTOs.MasterTopicDTOs
{
    public class MasterTopicResponse
    {
        public Guid Id { get; set; }
        public DomainResponse Domain { get; set; } = null!;
        public SemesterResponse Semester { get; set; } = null!;
        public List<LecturerResponse> Lecturers { get; set; } = new List<LecturerResponse>(); // Changed from single Lecturer to List
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public StaticEnum.StatusEnum Status { get; set; } = StaticEnum.StatusEnum.Active; // active | disabled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class DomainResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public StaticEnum.StatusEnum Status { get; set; } = StaticEnum.StatusEnum.Active; // active | disabled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class SemesterResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public StaticEnum.SemesterTermEnum Term { get; set; }
        public int Year { get; set; }
        public StaticEnum.SemesterStatusEnum Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class LecturerResponse
    {
        public Guid LecturerId { get; set; }
        public string? FullName { get; set; }
        public StaticEnum.GenderEnum Gender { get; set; }
        public StaticEnum.StatusEnum Status { get; set; }
    }
}