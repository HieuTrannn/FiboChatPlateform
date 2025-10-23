using static Course.Domain.Enums.StaticEnums;

namespace Course.Application.DTOs.DocumentDTOs
{
    public class DocumentTypeResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DocumentTypeStatus Status { get; set; } = DocumentTypeStatus.Active; // active | disabled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}