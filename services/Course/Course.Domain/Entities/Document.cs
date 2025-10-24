using static Course.Domain.Enums.StaticEnums;

namespace Course.Domain.Entities
{
    public class Document
    {
        public Guid Id { get; set; }
        public Guid TopicId { get; set; }
        public Guid DocumentTypeId { get; set; }
        public Guid FileId { get; set; }
        public string Title { get; set; } = null!;
        public int Version { get; set; }
        public DocumentStatus Status { get; set; } = DocumentStatus.Draft; // draft | published
        public Guid VerifiedById { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // navigation
        public Topic Topic { get; set; } = null!;
        public DocumentType DocumentType { get; set; } = null!;
        public File File { get; set; } = null!;
        public ICollection<Embedding> Embeddings { get; set; } = new List<Embedding>();
    }
}