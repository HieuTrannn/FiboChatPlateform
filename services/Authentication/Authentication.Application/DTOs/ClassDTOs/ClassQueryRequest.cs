using Authentication.Domain.Enum;

namespace Authentication.Application.DTOs.ClassDTOs
{
    /// <summary>
    /// Query parameters for class search, filtering, and sorting
    /// </summary>
    public class ClassQueryRequest
    {
        /// <summary>
        /// Search term for class code
        /// </summary>
        public string? Search { get; set; }
        
        /// <summary>
        /// Filter by class status
        /// </summary>
        public ClassStatusEnum? Status { get; set; }
        
        /// <summary>
        /// Filter by semester ID
        /// </summary>
        public Guid? SemesterId { get; set; }
        
        /// <summary>
        /// Filter by lecturer ID
        /// </summary>
        public Guid? LecturerId { get; set; }
        
        /// <summary>
        /// Sort field (Code, CreatedAt, Status)
        /// </summary>
        public string? SortBy { get; set; } = "CreatedAt";
        
        /// <summary>
        /// Sort direction (asc, desc)
        /// </summary>
        public string? SortDirection { get; set; } = "desc";
        
        /// <summary>
        /// Page number (1-based)
        /// </summary>
        public int Page { get; set; } = 1;
        
        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}