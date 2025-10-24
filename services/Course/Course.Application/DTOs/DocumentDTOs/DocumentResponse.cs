using Contracts.Common;
using static Course.Domain.Enums.StaticEnums;

namespace Course.Application.DTOs.DocumentDTOs
{
    public class DocumentResponse
    {
        public Guid Id { get; set; }
        public Guid TopicId { get; set; }
        public Guid DocumentTypeId { get; set; }
        public Guid FileId { get; set; }
        public string Title { get; set; } = null!;
        public int Version { get; set; }
        public DocumentStatus Status { get; set; }
        public Guid VerifiedById { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public TopicResponse Topic { get; set; } = null!;
        public DocumentTypeResponse DocumentType { get; set; } = null!;
        public FileResponse File { get; set; } = null!;
    }

    public class TopicResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid MasterTopicId { get; set; }
        public StaticEnum.StatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class FileResponse
    {
        public Guid Id { get; set; }
        public Guid OwnerUserId { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string FileContentType { get; set; } = null!;
        public long FileSize { get; set; }
        public string FileUrl { get; set; } = null!;
        public string FileKey { get; set; } = null!;
        public string FileBucket { get; set; } = null!;
        public string FileRegion { get; set; } = null!;
        public string FileAcl { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; } = null!;
    }
}