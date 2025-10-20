using Authentication.Domain.Enum;

namespace Authentication.Application.DTOs.SemesterDTOs
{
    public class SemesterQueryRequest
    {
        public string? Search { get; set; }
        public SemesterStatusEnum? Status { get; set; }
        public int? Year { get; set; }
        public SemesterTermEnum? Term { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public string? SortDirection { get; set; } = "desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
