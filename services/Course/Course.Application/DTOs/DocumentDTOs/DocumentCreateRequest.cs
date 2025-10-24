namespace Course.Application.DTOs.DocumentDTOs
{
    public class DocumentCreateRequest
    {
        public Guid TopicId { get; set; }
        public Guid DocumentTypeId { get; set; }
        public string Title { get; set; } = null!;
        public int Version { get; set; } = 1;
        public Guid VerifiedById { get; set; }
        public Guid UpdatedById { get; set; }
    }
}