using static Course.Domain.Enums.StaticEnums;

namespace Course.Domain.Entities
{
    public class DocumentType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DocumentTypeStatus Status { get; set; } = DocumentTypeStatus.Active; // active | disabled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // navigation
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}