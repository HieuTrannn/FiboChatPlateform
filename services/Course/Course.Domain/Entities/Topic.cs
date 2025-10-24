using Contracts.Common;

namespace Course.Domain.Entities
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid MasterTopicId { get; set; }
        public StaticEnum.StatusEnum Status { get; set; } = StaticEnum.StatusEnum.Active; // active | disabled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // navigation
        public MasterTopic MasterTopic { get; set; } = null!;
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}