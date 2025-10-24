using Microsoft.AspNetCore.Http;


namespace Course.Application.DTOs.DocumentDTOs
{
    public class DocumentUploadRequest
    {
        public Guid TopicId { get; set; }
        public Guid DocumentTypeId { get; set; }
        public string Title { get; set; } = null!;
        public Guid VerifiedById { get; set; }
        public Guid UpdatedById { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}