using static Course.Domain.Enums.StaticEnums;
namespace Course.Application.DTOs.DocumentDTOs
{
    public class DocumentUpdateRequest
    {
        public string? Title { get; set; }
        public int? Version { get; set; }
        public DocumentStatus? Status { get; set; }
        public Guid? VerifiedById { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}