namespace Course.Domain.Entities
{
    public class File
    {
        public Guid Id { get; set; }
        public Guid OwnerUserId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; } 
        public string FileContentType { get; set; }
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "active";
        public string FileUrl { get; set; }
        public string FileKey { get; set; }
        public string FileBucket { get; set; }
        public string FileRegion { get; set; }
        public string FileAcl { get; set; }
    }
}