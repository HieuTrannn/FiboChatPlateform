using Authentication.Domain.Enum;

namespace Authentication.Application.DTOs.GroupDTOs
{
    public class GroupQueryRequest
    {
        public string? Search { get; set; }
        public Guid? ClassId { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public string? SortDirection { get; set; } = "desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
