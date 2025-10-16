using System.ComponentModel.DataAnnotations.Schema;

namespace Course.Domain.Entities
{
    public class MasterTopicKeyword
    {
        public Guid Id { get; set; }
        public Guid MasterTopicId { get; set; }
        public Guid KeywordId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("MasterTopicId")]
        public MasterTopic MasterTopic { get; set; } = null!;
        [ForeignKey("KeywordId")]
        public Keyword Keyword { get; set; } = null!;
    }
}